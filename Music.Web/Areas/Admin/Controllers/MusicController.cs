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
				model.BandDescption = band.Description;
				model.BandImage = band.ImageUrl;

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



		// GET: Admin/Music/AlbumType
		[HttpGet]
		[Authorize(Roles = "admin, editor")]
		public async Task<ActionResult> AlbumType(int bandId, string albumTypeName)
		{
			var albumType = new AlbumType();

			if (!string.IsNullOrEmpty(albumTypeName))
			{
				albumType = await _albumTypeRepository.GetByNameAsync(albumTypeName);
			}

			var model = new MusicViewModel();

			if (albumType != null)
			{
				model.AlbumTypeOldName = albumType.Name;
				model.AlbumTypeDescption = albumType.Description;
				model.AlbumTypeImage = albumType.ImageUrl;
				model.BandId = bandId;

				model.AlbumTypes = await _albumTypeRepository.GetAllyAsync();
				model.Bands = await _bandRepository.GetAllyAsync();
			}
			else
			{
				model.AlbumTypes = await _albumTypeRepository.GetAllyAsync();
				model.Bands = await _bandRepository.GetAllyAsync();
			}

			return View(model);
		}

		// Post: Admin/Music/AlbumType
		[HttpPost]
		[Authorize(Roles = "admin, editor")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AlbumType(MusicViewModel albumType, HttpPostedFileBase file)
		{
			var model = new MusicViewModel
			{
				AlbumTypes = await _albumTypeRepository.GetAllyAsync(),
				Bands = await _bandRepository.GetAllyAsync()
			};
			var albumTypeCheck = new AlbumType();
			var allowedExtensions = new[] {
				".Jpg", ".png", ".jpg", "jpeg"
			};

			try
			{
				if (!ModelState.IsValid)
				{
					ModelState.AddModelError(string.Empty, "لطفا مقدار های مناسب پر کنید");
				}
				if (string.IsNullOrEmpty(albumType.AlbumTypeOldName))
				{
					albumType.AlbumTypeOldName = albumType.AlbumTypeNewName;
				}

				albumTypeCheck = await _albumTypeRepository.GetByNameAsync(albumType.AlbumTypeOldName);

				if (albumType.ActionType == "create" || albumType.ActionType == "edit")
				{
					if (albumTypeCheck == null)
					{
						if (file != null)
						{
							var fileName = Path.GetFileName(file.FileName);
							var ext = Path.GetExtension(file.FileName);
							if (allowedExtensions.Contains(ext))
							{
								string name = Path.GetFileNameWithoutExtension(fileName);
								string myfile = name + "_" + albumType.AlbumTypeNewName + ext;
								var path = Path.Combine(Server.MapPath("~/DownloadCenter/AlbumType"), myfile);
								albumType.AlbumTypeImage = "~/DownloadCenter/AlbumType/" + myfile;
								file.SaveAs(path);
							}
							else
							{
								ModelState.AddModelError(string.Empty, "Please choose only Image file");
							}
						}

						var AlbumTypemodel = new AlbumType
						{
							Name = albumType.AlbumTypeOldName,
							Description = albumType.AlbumTypeDescption,
							ImageUrl = albumType.AlbumTypeImage,
							BandId = albumType.BandId
						};


						await _albumTypeRepository.CreateAsync(AlbumTypemodel);

						model.AlbumTypes = await _albumTypeRepository.GetAllyAsync();
						model.BandId = albumType.BandId;
						//return RedirectToAction("Section", new { surveyName = surveys.SurveyTitle });
						return View(model);
					}
					else
					{
						albumTypeCheck.Name = (albumType.AlbumTypeOldName == albumType.AlbumTypeNewName ? albumType.AlbumTypeOldName : albumType.AlbumTypeNewName);
						albumTypeCheck.Description = albumType.AlbumTypeDescption;
						albumTypeCheck.ImageUrl = albumType.AlbumTypeImage;
						albumTypeCheck.BandId = albumType.BandId;

						await _albumTypeRepository.EditAsync(albumTypeCheck.Id, albumTypeCheck);
					}
				}
				else
				{
					await _albumTypeRepository.DeleteAsync(albumTypeCheck.Id);
				}

				model.AlbumTypes = await _albumTypeRepository.GetAllyAsync();
				model.BandId = albumType.BandId;

				//return RedirectToAction("Section", new { surveyName = model.GenreOldName });
				return View(model);
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
				return View(model: model);
			}
		}



		// GET: Admin/Music/Album
		[HttpGet]
		[Authorize(Roles = "admin, editor")]
		public async Task<ActionResult> Album(int albumTypeId, string albumName)
		{
			var album = new Album();

			if (!string.IsNullOrEmpty(albumName))
			{
				album = await _albumRepository.GetByNameAsync(albumName);
			}

			var model = new MusicViewModel();

			if (album != null)
			{
				model.AlbumOldName = album.Name;
				model.AlbumDescption = album.Description;
				model.AlbumImage = album.ImageUrl;
				model.AlbumTypeId = albumTypeId;

				model.Albums = await _albumRepository.GetAllyAsync();
				model.AlbumTypes = await _albumTypeRepository.GetAllyAsync();
			}
			else
			{
				model.Albums = await _albumRepository.GetAllyAsync();
				model.AlbumTypes = await _albumTypeRepository.GetAllyAsync();
			}

			return View(model);
		}

		// Post: Admin/Music/Album
		[HttpPost]
		[Authorize(Roles = "admin, editor")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Album(MusicViewModel album, HttpPostedFileBase file)
		{
			var model = new MusicViewModel
			{
				Albums = await _albumRepository.GetAllyAsync(),
				AlbumTypes = await _albumTypeRepository.GetAllyAsync()
			};
			var albumCheck = new Album();
			var allowedExtensions = new[] {
				".Jpg", ".png", ".jpg", "jpeg"
			};

			try
			{
				if (!ModelState.IsValid)
				{
					ModelState.AddModelError(string.Empty, "لطفا مقدار های مناسب پر کنید");
				}
				if (string.IsNullOrEmpty(album.AlbumOldName))
				{
					album.AlbumOldName = album.AlbumNewName;
				}

				albumCheck = await _albumRepository.GetByNameAsync(album.AlbumOldName);

				if (album.ActionType == "create" || album.ActionType == "edit")
				{
					if (albumCheck == null)
					{
						if (file != null)
						{
							var fileName = Path.GetFileName(file.FileName);
							var ext = Path.GetExtension(file.FileName);
							if (allowedExtensions.Contains(ext))
							{
								string name = Path.GetFileNameWithoutExtension(fileName);
								string myfile = name + "_" + album.AlbumNewName + ext;
								var path = Path.Combine(Server.MapPath("~/DownloadCenter/Album"), myfile);
								album.AlbumImage = "~/DownloadCenter/Album/" + myfile;
								file.SaveAs(path);
							}
							else
							{
								ModelState.AddModelError(string.Empty, "Please choose only Image file");
							}
						}

						var albummodel = new Album
						{
							Name = album.AlbumOldName,
							Description = album.AlbumDescption,
							ImageUrl = album.AlbumImage,
							AlbumTypeId = album.AlbumTypeId
						};


						await _albumRepository.CreateAsync(albummodel);

						model.Albums = await _albumRepository.GetAllyAsync();
						model.AlbumTypeId = album.AlbumTypeId;

						//return RedirectToAction("Section", new { surveyName = surveys.SurveyTitle });
						return View(model);
					}
					else
					{
						albumCheck.Name = (album.AlbumOldName == album.AlbumNewName ? album.AlbumOldName : album.AlbumNewName);
						albumCheck.Description = album.AlbumDescption;
						albumCheck.ImageUrl = album.AlbumImage;
						albumCheck.AlbumTypeId = album.AlbumTypeId;

						await _albumRepository.EditAsync(albumCheck.Id, albumCheck);
					}
				}
				else
				{
					await _albumRepository.DeleteAsync(albumCheck.Id);
				}

				model.Albums = await _albumRepository.GetAllyAsync();
				model.AlbumTypeId = album.AlbumTypeId;

				//return RedirectToAction("Section", new { surveyName = model.GenreOldName });
				return View(model);
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
				return View(model: model);
			}
		}

		// GET: Admin/Music/File
		[HttpGet]
		[Authorize(Roles = "admin, editor")]
		public async Task<ActionResult> File(int albumId, string fileName)
		{
			var file = new Music.Core.Entities.File();

			if (!string.IsNullOrEmpty(fileName))
			{
				file = await _fileRepository.GetByNameAsync(fileName);
			}

			var model = new MusicViewModel();

			if (file != null)
			{
				model.FileOldName = file.FileName;
				model.FileRoot = file.FileRoot;
				model.FileSize = file.FileSize;
				model.FileType = file.FileType;
				model.FileDescption = file.Description;
				model.FileImage = file.ImageUrl;
				model.AlbumId = albumId;

				model.Files = await _fileRepository.GetAllyAsync();
				model.Albums = await _albumRepository.GetAllyAsync();
			}
			else
			{
				model.Files = await _fileRepository.GetAllyAsync();
				model.Albums = await _albumRepository.GetAllyAsync();
			}

			return View(model);
		}

		// Post: Admin/Music/File
		[HttpPost]
		[Authorize(Roles = "admin, editor")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> File(MusicViewModel file, HttpPostedFileBase image, HttpPostedFileBase music)
		{
			var model = new MusicViewModel
			{
				Files = await _fileRepository.GetAllyAsync(),
				Albums = await _albumRepository.GetAllyAsync()
			};
			var fileCheck = new Music.Core.Entities.File();
			var allowedExtensionsImage = new[] {
				".Jpg", ".png", ".jpg", "jpeg"
			};
			var allowedExtensionsMusic = new[] {
				".mp3", ".MP3"
			};

			var user = await GetloggedInUser();

			try
			{
				if (!ModelState.IsValid)
				{
					ModelState.AddModelError(string.Empty, "لطفا مقدار های مناسب پر کنید");
				}
				if (string.IsNullOrEmpty(file.FileOldName))
				{
					file.FileOldName = file.FileNewName;
				}

				fileCheck = await _fileRepository.GetByNameAsync(file.FileOldName);

				if (file.ActionType == "create" || file.ActionType == "edit")
				{
					if (fileCheck == null)
					{
						if (image != null)
						{
							var fileName = Path.GetFileName(image.FileName);
							var ext = Path.GetExtension(image.FileName);
							if (allowedExtensionsImage.Contains(ext))
							{
								string name = Path.GetFileNameWithoutExtension(fileName);
								string myfile = name + "_" + file.FileNewName + ext;
								var path = Path.Combine(Server.MapPath("~/DownloadCenter/File"), myfile);
								file.FileImage = "~/DownloadCenter/File/" + myfile;
								file.FileType = ext;
								file.FileSize = music.ContentLength;
								image.SaveAs(path);
							}
							else
							{
								ModelState.AddModelError(string.Empty, "Please choose only Image file");
							}
						}
						if (music != null)
						{
							var fileName = Path.GetFileName(music.FileName);
							var ext = Path.GetExtension(music.FileName);
							if (allowedExtensionsMusic.Contains(ext))
							{
								string name = Path.GetFileNameWithoutExtension(fileName);
								string myfile = name + "_" + file.FileNewName + ext;
								var path = Path.Combine(Server.MapPath("~/DownloadCenter/File"), myfile);
								file.FileRoot = "~/DownloadCenter/File/" + myfile;
								file.FileType = ext;
								file.FileSize = music.ContentLength;
								
								music.SaveAs(path);
							}
							else
							{
								ModelState.AddModelError(string.Empty, "Please choose only Music file");
							}
						}
						var filemodel = new Music.Core.Entities.File
						{
							FileName = file.FileNewName,
							FileRoot = file.FileRoot,
							FilePath = file.FileRoot,
							FileType = file.FileType,
							FileSize = file.FileSize,
							Description = file.FileDescption,
							ImageUrl = file.FileImage,
							AlbumId = file.AlbumId,
							AuthorId = user.Id
						};


						await _fileRepository.CreateAsync(filemodel);

						model.Files = await _fileRepository.GetAllyAsync();
						model.AlbumId = file.AlbumId;

						//return RedirectToAction("Section", new { surveyName = surveys.SurveyTitle });
						return View(model);
					}
					else
					{
						fileCheck.FileName = (file.FileOldName == file.FileNewName ? file.FileOldName : file.FileNewName);
						fileCheck.FileRoot = file.FileRoot;
						fileCheck.FilePath = file.FileRoot;
						fileCheck.FileType = file.FileType;
						fileCheck.FileSize= file.FileSize;
						fileCheck.Description = file.FileDescption;
						fileCheck.ImageUrl = file.FileImage;
						fileCheck.AlbumId = file.AlbumId;
						fileCheck.AuthorId = user.Id;

						await _fileRepository.EditAsync(fileCheck.Id, fileCheck);
					}
				}
				else
				{
					await _fileRepository.DeleteAsync(fileCheck.Id);
				}

				model.Files = await _fileRepository.GetAllyAsync();
				model.AlbumId = file.AlbumId;

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