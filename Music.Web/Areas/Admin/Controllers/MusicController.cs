using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Music.Core.Entities;
using Music.Core.Interfaces;
using Music.Infrastructure.Repository;
using Music.Web.Areas.Admin.ViewModels;

namespace Music.Web.Areas.Admin.Controllers
{
	[RouteArea("admin")]
	[RoutePrefix("Muisc")]
	[Authorize]
	//[Authorize(Roles = "admin, editor, author")]
	public class MusicController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly IGenreRepository _genreRepository;
		private readonly IBandRepository _bandRepository;
		private readonly IAlbumTypeRepository _albumTypeRepository;
		private readonly IAlbumRepository _albumRepository;
		private readonly IFileRepository _fileRepository;

		public MusicController()
			: this(new UserRepository(), new GenreRepository(), new BandRepository(),
				 new AlbumTypeRepository(), new AlbumRepository(), new FileRepository())
		{

		}

		public MusicController(IUserRepository userRepository, IGenreRepository genreRepository, IBandRepository bandRepository,
								IAlbumTypeRepository albumTypeRepository, IAlbumRepository albumRepository, IFileRepository fileRepository)
		{
			_userRepository = userRepository;
			_genreRepository = genreRepository;
			_bandRepository = bandRepository;
			_albumTypeRepository = albumTypeRepository;
			_albumRepository = albumRepository;
			_fileRepository = fileRepository;
		}

		// GET: Admin/Music/Genre
		[HttpGet]
		[Authorize(Roles = "admin, editor")]
		public async Task<ActionResult> Genre(string genreName)
		{
			var genre = await _genreRepository.GetByNameAsync(genreName);
			var model = new MusicViewModel();

			if (genre != null)
			{
				model.GenreOldName = genre.Name;
				model.Genres = await _genreRepository.GetAllyAsync();
			}
			else
			{
				model.Genres = await _genreRepository.GetAllyAsync();
			}

			return View(model);
		}
		// Post: Admin/Music/Genre
		[HttpPost]
		[Authorize(Roles = "admin, editor")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Genre(MusicViewModel genre)
		{
			var model = new MusicViewModel() { Genres = await _genreRepository.GetAllyAsync() };
			var genreModel = new Genre();
			try
			{
				if (!ModelState.IsValid)
				{
					ModelState.AddModelError(string.Empty, "لطفا مقدار های مناسب پر کنید");
				}
				if (string.IsNullOrEmpty(genre.GenreOldName))
				{
					genre.GenreOldName = genre.GenreNewName;
				}

				genreModel = await _genreRepository.GetByNameAsync(genre.GenreOldName);

				if (genre.ActionType == "create" || genre.ActionType == "edit")
				{
					if (genreModel == null)
					{
						await _genreRepository.CreateAsync(new Genre
						{
							Name = genre.GenreOldName,
						});
						//return RedirectToAction("Section", new { surveyName = surveys.SurveyTitle });
						return View();
					}
					else
					{
						genreModel.Name = (genre.GenreOldName == genre.GenreNewName ? genre.GenreOldName : genre.GenreNewName);

						await _genreRepository.EditAsync(genreModel.Id, genreModel);
					}
					model.GenreOldName = genreModel.Name;
				}
				else
				{
					await _genreRepository.DeleteAsync(genreModel.Id);
				}
				

				//return RedirectToAction("Section", new { surveyName = model.GenreOldName });
				return View();
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
				return View(model: model);
			}
		}


		#region Method

		private bool _isDisposed;
		protected override void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				_userRepository.Dispose();
			}

			_isDisposed = true;
			base.Dispose(disposing);
		}

		private async Task<UserIdentity> GetloggedInUser()
		{
			return await _userRepository.GetUserByNameAsync(User.Identity.Name);
		}

		#endregion
	}
}