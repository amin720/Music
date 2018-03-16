using Music.Core.Entities;
using Music.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Music.Infrastructure.Repository
{
	public class AlbumRepository : IAlbumRepository
	{
		public async Task<Album> GetByIdAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var album = await db.Albums.SingleOrDefaultAsync(p => p.Id == id);

				if (album == null)
				{
					throw new KeyNotFoundException("The Album Id \"" + id + "\" does not exist.");
				}

				return album;
			}
		}
		public async Task<Album> GetByNameAsync(string name)
		{
			using (var db = new MusicUniEntities())
			{
				var album = await db.Albums.SingleOrDefaultAsync(p => p.Name == name);

				if (album == null)
				{
					throw new KeyNotFoundException("The Album name \"" + name + "\" does not exist.");
				}

				return album;
			}
		}
		public async Task<IEnumerable<Album>> GetAllyAsync()
		{
			using (var db = new MusicUniEntities())
			{
				var album = await db.Albums.OrderBy(g => g.Name)
											   .ToListAsync();

				return album;
			}
		}
		public async Task CreateAsync(Album model)
		{
			using (var db = new MusicUniEntities())
			{
				var album =
					await db.Albums.SingleOrDefaultAsync(pro => pro.Name == model.Name);

				if (album != null)
				{
					throw new ArgumentException("A album with the id of " + model.Id + " already exsits.");
				}

				db.Albums.Add(model);
				await db.SaveChangesAsync();
			}
		}

		public async Task EditAsync(int id, Album updateItem)
		{
			using (var db = new MusicUniEntities())
			{
				var album = await db.Albums.SingleOrDefaultAsync(p => p.Id == id);

				if (album == null)
				{
					throw new KeyNotFoundException("A album withthe id of " + id + "does not exisst in the data store");
				}

				album.Name = updateItem.Name;
				album.Description = updateItem.Description;
				album.ImageUrl = updateItem.ImageUrl;
				album.AlbumTypeId = updateItem.AlbumTypeId;

				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var album = await db.Albums.SingleOrDefaultAsync(p => p.Id == id);

				if (album == null)
				{
					throw new KeyNotFoundException("The album with the id of " + id + "does not exist.");
				}

				db.Albums.Remove(album);
				await db.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Album>> GetPageByAlbumTypeAsync(int pageNumber, int pageSize, int albumTypeId)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.Albums.Where(p => p.AlbumTypeId == albumTypeId)
										  .OrderByDescending(p => p.Name)
										  .Skip(skip)
										  .Take(pageSize)
										  .ToArrayAsync();
			}
		}
		public async Task<IEnumerable<Album>> GetPageByAlbumTypeAsync(int pageNumber, int pageSize, string albumTypeName)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.Albums.Where(p => p.AlbumType.Name == albumTypeName)
										  .OrderByDescending(p => p.Name)
										  .Skip(skip)
										  .Take(pageSize)
										  .ToArrayAsync();
			}
		}

		public async Task<IEnumerable<Album>> GetPageAsync(int pageNumber, int pageSize)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.Albums
										.OrderByDescending(p => p.Name)
										.Skip(skip)
										.Take(pageSize)
										.ToArrayAsync();
			}
		}

		//public async Task<IEnumerable<Album>> GetAlbumsByAuthorAsync(string authorId)
		//{
		//	using (var db = new MusicUniEntities())
		//	{
		//		return await db.Albums.Include("AspNetUser")
		//								.Where(p => p.AuthorId == authorId)
		//								.OrderByDescending(post => post.Published)
		//								.ToArrayAsync();
		//	}
		//}
	}
}
