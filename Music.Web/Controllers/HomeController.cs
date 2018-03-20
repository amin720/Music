using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Music.Core.Interfaces;
using Music.Infrastructure.Repository;
using Music.Web.Services;

namespace Music.Web.Controllers
{
	[AllowAnonymous]
	[RoutePrefix("")]
	public class HomeController : Controller
	{
		private readonly IGenreRepository _genreRepository;
		private readonly IBandRepository _bandRepository;
		private readonly IAlbumTypeRepository _albumTypeRepository;
		private readonly IAlbumRepository _albumRepository;
		private readonly IFileRepository _fileRepository;

		public HomeController()
			: this(new GenreRepository(), new BandRepository(),
				   new AlbumTypeRepository(), new AlbumRepository(), new FileRepository())
		{
		}

		public HomeController(IGenreRepository genreRepository, IBandRepository bandRepository,
							  IAlbumTypeRepository albumTypeRepository, IAlbumRepository albumRepository, IFileRepository fileRepository)
		{
			_genreRepository = genreRepository;
			_bandRepository = bandRepository;
			_albumTypeRepository = albumTypeRepository;
			_albumRepository = albumRepository;
			_fileRepository = fileRepository;
		}


		// GET: Home
		public async Task<ActionResult> Index()
		{
			var lastRelease = await _fileRepository.GetPageAsync(1, 4);


			ViewBag.ListGenres = await _genreRepository.GetAllyAsync();
			ViewBag.LastRelease = lastRelease.Shuffle();
			ViewBag.LastReleaseSpecial = lastRelease.Shuffle();

			return View();
		}

		// GET: Bands
		[HttpGet]
		[Route("Bands/{genreId}")]
		public async Task<ActionResult> Bands(int genreId)
		{
			var genre = await _genreRepository.GetByIdAsync(genreId);

			ViewBag.GenreName = genre.Name;
			ViewBag.ListBands = await _bandRepository.GetPageByGenreAsync(1, 12, genre.Name);

			return View();
		}

		// GET: Profile
		[HttpGet]
		[Route("Profile/{bandId}")]
		public async Task<ActionResult> Profile(int bandId)
		{
			var band = await _bandRepository.GetByIdAsync(bandId);

			ViewBag.ImageBand = band.ImageUrl;
			ViewBag.NameBand = band.Name;
			ViewBag.DescriptionBand = band.Description;

			ViewBag.ListAlbumType = await _albumTypeRepository.GetPageByBandAsync(1, 12, band.Name);
			ViewBag.ListAlbum = await _albumRepository.GetPageByAlbumTypeAsync(1, 12, band.Name);

			return View();
		}

		// GET: Profile
		[HttpGet]
		public async Task<ActionResult> Album(int bandId,int albumTypeId)
		{
			var band = await _bandRepository.GetByIdAsync(bandId);
			var albumType = await _albumTypeRepository.GetByIdAsync(albumTypeId);

			ViewBag.ImageBand = band.ImageUrl;
			ViewBag.NameBand = band.Name;
			ViewBag.DescriptionBand = band.Description;

			ViewBag.ListAlbumType = await _albumTypeRepository.GetPageByBandAsync(1, 12, band.Name);
			ViewBag.ListAlbum = await _albumRepository.GetPageByAlbumTypeAsync(1, 12, band.Name);

			return View();
		}
	}
}