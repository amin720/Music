using Music.Core.Entities;
using Music.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Music.Infrastructure.Repository
{
	public class FileRepository : IFileRepository
	{
		public async Task<File> GetByIdAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var file = await db.Files.SingleOrDefaultAsync(p => p.Id == id);

				//if (file == null)
				//{
				//	throw new KeyNotFoundException("The File Id \"" + id + "\" does not exist.");
				//}

				return file;
			}
		}
		public async Task<File> GetByNameAsync(string fileName)
		{
			using (var db = new MusicUniEntities())
			{
				var file = await db.Files.SingleOrDefaultAsync(p => p.FileName == fileName);

				//if (file == null)
				//{
				//	throw new KeyNotFoundException("The File name \"" + fileName + "\" does not exist.");
				//}

				return file;
			}
		}
		public async Task<File> GetByAlbumAsync(int albumId, string fileName)
		{
			using (var db = new MusicUniEntities())
			{
				var file = await db.Files.SingleOrDefaultAsync(p => p.FileName == fileName && p.AlbumId == albumId);

				

				return file;
			}
		}
		public async Task<IEnumerable<File>> GetAllyAsync()
		{
			using (var db = new MusicUniEntities())
			{
				var file = await db.Files.OrderBy(g => g.FileName)
											   .ToListAsync();

				return file;
			}
		}
		public async Task CreateAsync(File model)
		{
			using (var db = new MusicUniEntities())
			{
				var file =
					await db.Files.SingleOrDefaultAsync(pro => pro.FileName == model.FileName && pro.AlbumId == model.AlbumId);

				if (file != null)
				{
					throw new ArgumentException("A file with the id of " + model.Id + " already exsits.");
				}

				db.Files.Add(model);
				await db.SaveChangesAsync();
			}
		}
		public async Task EditAsync(int id, File updateItem)
		{
			using (var db = new MusicUniEntities())
			{
				var file = await db.Files.SingleOrDefaultAsync(p => p.Id == id);

				if (file == null)
				{
					throw new KeyNotFoundException("A file withthe id of " + id + "does not exisst in the data store");
				}

				file.Price = updateItem.Price;
				file.FileName = updateItem.FileName;
				file.FilePath = updateItem.FilePath;
				file.FileRoot = updateItem.FileRoot;
				file.FileSize = updateItem.FileSize;
				file.FileType = updateItem.FileType;
				file.Description = updateItem.Description;
				file.ImageUrl = updateItem.ImageUrl;
				file.AlbumId = updateItem.AlbumId;

				await db.SaveChangesAsync();
			}
		}
		public async Task DeleteAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var file = await db.Files.SingleOrDefaultAsync(p => p.Id == id);

				if (file == null)
				{
					throw new KeyNotFoundException("The file with the id of " + id + "does not exist.");
				}

				db.Files.Remove(file);
				await db.SaveChangesAsync();
			}
		}
		public async Task<IEnumerable<File>> GetPageByAlbumAsync(int pageNumber, int pageSize, int albumId)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.Files.Where(p => p.AlbumId == albumId)
										  .OrderByDescending(p => p.FileName)
										  .Skip(skip)
										  .Take(pageSize)
										  .ToArrayAsync();
			}
		}
		public async Task<IEnumerable<File>> GetPageByAlbumAsync(int pageNumber, int pageSize, string albumName)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.Files.Where(p => p.Album.Name == albumName)
										  .OrderByDescending(p => p.FileName)
										  .Skip(skip)
										  .Take(pageSize)
										  .ToArrayAsync();
			}
		}
		public async Task<IEnumerable<File>> GetPageAsync(int pageNumber, int pageSize)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.Files
										.OrderByDescending(p => p.FileName)
										.Skip(skip)
										.Take(pageSize)
										.ToArrayAsync();
			}
		}

		//public async Task<IEnumerable<File>> GetFilesByAuthorAsync(string authorId)
		//{
		//	using (var db = new MusicUniEntities())
		//	{
		//		return await db.Files.Include("AspNetUser")
		//								.Where(p => p.AuthorId == authorId)
		//								.OrderByDescending(post => post.Published)
		//								.ToArrayAsync();
		//	}
		//}
	}
}
