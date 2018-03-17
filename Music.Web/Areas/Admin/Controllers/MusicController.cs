using System;
using System.Collections.Generic;
using System.IO;
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
			var genre = new Genre();
			if (!string.IsNullOrEmpty(genreName))
			{
				genre = await _genreRepository.GetByNameAsync(genreName);
			}

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
						model.Genres = await _genreRepository.GetAllyAsync();
						//return RedirectToAction("Section", new { surveyName = surveys.SurveyTitle });
						return View(model);
					}
					else
					{
						genreModel.Name = (genre.GenreOldName == genre.GenreNewName ? genre.GenreOldName : genre.GenreNewName);

						await _genreRepository.EditAsync(genreModel.Id, genreModel);
					}
					
				}
				else
				{
					await _genreRepository.DeleteAsync(genreModel.Id);
				}

				model.Genres = await _genreRepository.GetAllyAsync();

				//return RedirectToAction("Section", new { surveyName = model.GenreOldName });
				return View(model);
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
				return View(model: model);
			}
		}

		// GET: Admin/Music/Band
		[HttpGet]
		[Authorize(Roles = "admin, editor")]
		public async Task<ActionResult> Band(string bandName)
		{
			var band = new Band();

			if (!string.IsNullOrEmpty(bandName))
			{
				band = await _bandRepository.GetByNameAsync(bandName);
			}
			
			var model = new MusicViewModel();

			if (band != null)
			{
				model.BandOldName = band.Name;
				model.Bands = await _bandRepository.GetAllyAsync();
				model.Genres = await _genreRepository.GetAllyAsync();
			}
			else
			{
				model.Genres = await _genreRepository.GetAllyAsync();
				model.Bands = await _bandRepository.GetAllyAsync();
			}

			return View(model);
		}

		// Post: Admin/Music/Band
		[HttpPost]
		[Authorize(Roles = "admin, editor")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Band(MusicViewModel band, HttpPostedFileBase file)
		{
			var model = new MusicViewModel
			{
				Bands = await _bandRepository.GetAllyAsync(),
				Genres = await _genreRepository.GetAllyAsync()
			};
			var bandCheck = new Band();
			var allowedExtensions = new[] {
				".Jpg", ".png", ".jpg", "jpeg"
			};
			var genre = await _genreRepository.GetByIdAsync(band.GenreId);

			try
			{
				if (!ModelState.IsValid)
				{
					ModelState.AddModelError(string.Empty, "لطفا مقدار های مناسب پر کنید");
				}
				if (string.IsNullOrEmpty(band.GenreOldName))
				{
					band.BandOldName = band.BandNewName;
				}

				bandCheck = await _bandRepository.GetByNameAsync(band.BandOldName);

				if (band.ActionType == "create" || band.ActionType == "edit")
				{
					if (bandCheck == null)
					{
						if (file != null)
						{
							var fileName = Path.GetFileName(file.FileName);
							var ext = Path.GetExtension(file.FileName);
							if (allowedExtensions.Contains(ext))
							{
								string name = Path.GetFileNameWithoutExtension(fileName);
								string myfile = name + "_" + band.BandNewName + ext;
								var path = Path.Combine(Server.MapPath("~/DownloadCenter/Band"), myfile);
								band.BandImage = "~/DownloadCenter/Band/" + myfile;
								file.SaveAs(path);
							}
							else
							{
								ModelState.AddModelError(string.Empty, "Please choose only Image file");
							}
						}

						var bandmodel = new Band
						{
							Name = band.BandOldName,
							Description = band.BandDescption,
							ImageUrl = band.BandImage
						};
						bandmodel.Genres.Add(genre);

						await _bandRepository.CreateAsync(bandmodel);

						model.Bands = await _bandRepository.GetAllyAsync();

						//return RedirectToAction("Section", new { surveyName = surveys.SurveyTitle });
						return View(model);
					}
					else
					{
						bandCheck.Name = (band.BandOldName == band.BandNewName ? band.BandOldName : band.BandNewName);
						bandCheck.Description = band.BandDescption;
						bandCheck.ImageUrl = band.BandImage;
						bandCheck.Genres.Add(genre);

						await _bandRepository.EditAsync(bandCheck.Id, bandCheck);
					}
				}
				else
				{
					await _bandRepository.DeleteAsync(bandCheck.Id);
				}

				model.Bands = await _bandRepository.GetAllyAsync();

				//return RedirectToAction("Section", new { surveyName = model.GenreOldName });
				return View(model);
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