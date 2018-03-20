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

		// GET: Home
		public ActionResult FAQ()
		{

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

			ViewBag.IdBand = bandId;
			ViewBag.ImageBand = band.ImageUrl;
			ViewBag.NameBand = band.Name;
			ViewBag.DescriptionBand = band.Description;

			return View();
		}

		// GET: Profile
		[HttpGet]
		[Route("Profile/{bandId}")]
		public async Task<ActionResult> AlbumType(int bandId)
		{
			var band = await _bandRepository.GetByIdAsync(bandId);

			ViewBag.IdBand = bandId;
			ViewBag.ImageBand = band.ImageUrl;
			ViewBag.NameBand = band.Name;
			ViewBag.DescriptionBand = band.Description;

			ViewBag.ListAlbumType = await _albumTypeRepository.GetPageByBandAsync(1, 12, band.Name);

			return View();
		}

		// GET: Album
		[HttpGet]
		public async Task<ActionResult> Album(int? bandId, int? albumTypeId)
		{
			var albumType = await _albumTypeRepository.GetByIdAsync((int)albumTypeId);
			var band = await _bandRepository.GetByIdAsync((int)bandId);

			ViewBag.IdBand = bandId;
			ViewBag.NameBand = band.Name;

			ViewBag.IdAlbumType = albumTypeId;
			ViewBag.ImageAlbumType = albumType.ImageUrl;
			ViewBag.NameAlbumType = albumType.Name;
			ViewBag.DescriptionAlbumType = albumType.Description;

			ViewBag.ListAlbum = await _albumRepository.GetPageByAlbumTypeAsync(1, 12, albumType.Name);

			return View();
		}

		// GET: File
		[HttpGet]
		public async Task<ActionResult> File(int albumId)
		{
			var album = await _albumRepository.GetByIdAsync(albumId);

			ViewBag.IdAlbumType = album.AlbumTypeId;

			ViewBag.ImageAlbum = album.ImageUrl;
			ViewBag.NameAlbum = album.Name;
			ViewBag.DescriptionAlbum = album.Description;

			ViewBag.ListFile = await _fileRepository.GetPageByAlbumAsync(1, 12, album.Name);

			return View();
		}

		// GET: Buy
		[Route("/Buy/{id}")]
		public async Task<ActionResult> Buy(int Id)
		{
			var model = await _fileRepository.GetByIdAsync(Id);
			if (model.Price == 0 || model.Price == null)
			{
				TempData["Id"] = model.Id;
				return RedirectToAction("Download", "Home");
			}

			return View(model: model);
		}

		// GET: Download
		public async Task<FileResult> Download(int? Id)
		{
			var id = Id == 0 || Id == null ? (int)TempData["Id"] : Id;
			var model = await _fileRepository.GetByIdAsync((int)id);

			//byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(model.FileRoot));
			//string fileName = model.FileName;

			string contentType = string.Empty;

			if (model.FileRoot.Contains(".mp3"))
			{
				contentType = "application/mp3";
			}

			else if (model.FileRoot.Contains(".MP3"))
			{
				contentType = "application/MP3";
			}
			else
			{
				contentType = "application/wfa";
			}

			//return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
			//return File(fileBytes, contentType, fileName);
			return new FilePathResult(Server.MapPath(model.FileRoot), contentType);

		}
	}
}