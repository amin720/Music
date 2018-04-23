using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.Core.Entities;
using Music.Core.Interfaces;

namespace Music.Infrastructure.Repository
{
	public class AlbumTypeRepository: IAlbumTypeRepository
	{
		public async Task<AlbumType> GetByIdAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var albumType = await db.AlbumTypes.SingleOrDefaultAsync(p => p.Id == id);

				//if (albumType == null)
				//{
				//	throw new KeyNotFoundException("The AlbumType Id \"" + id + "\" does not exist.");
				//}

				return albumType;
			}
		}
		public async Task<AlbumType> GetByNameAsync(string name)
		{
			using (var db = new MusicUniEntities())
			{
				var albumType = await db.AlbumTypes.SingleOrDefaultAsync(p => p.Name == name);

				//if (albumType == null)
				//{
				//	throw new KeyNotFoundException("The AlbumType name \"" + name + "\" does not exist.");
				//}

				return albumType;
			}
		}
		public async Task<AlbumType> GetByBandAsync(int bandId, string albumTypeName)
		{
			using (var db = new MusicUniEntities())
			{
				var albumType = await db.AlbumTypes.SingleOrDefaultAsync(p => p.Name == albumTypeName && p.BandId == bandId);

				return albumType;
			}
		}

		public async Task<IEnumerable<AlbumType>> GetAllyAsync()
		{
			using (var db = new MusicUniEntities())
			{
				var albumType = await db.AlbumTypes.OrderBy(g => g.Name)
											   .ToListAsync();

				return albumType;
			}
		}
		public async Task CreateAsync(AlbumType model)
		{
			using (var db = new MusicUniEntities())
			{
				var albumType =
					await db.AlbumTypes.SingleOrDefaultAsync(pro => pro.Name == model.Name && pro.BandId == model.BandId);

				if (albumType != null)
				{
					throw new ArgumentException("A albumType with the id of " + model.Id + " already exsits.");
				}

				db.AlbumTypes.Add(model);
				await db.SaveChangesAsync();
			}
		}

		public async Task EditAsync(int id, AlbumType updateItem)
		{
			using (var db = new MusicUniEntities())
			{
				var albumType = await db.AlbumTypes.SingleOrDefaultAsync(p => p.Id == id);

				if (albumType == null)
				{
					throw new KeyNotFoundException("A albumType withthe id of " + id + "does not exisst in the data store");
				}

				albumType.Name = updateItem.Name;
				albumType.Description = updateItem.Description;
				albumType.ImageUrl = updateItem.ImageUrl;
				albumType.BandId = updateItem.BandId;

				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var albumType = await db.AlbumTypes.SingleOrDefaultAsync(p => p.Id == id);

				if (albumType == null)
				{
					throw new KeyNotFoundException("The albumType with the id of " + id + "does not exist.");
				}

				db.AlbumTypes.Remove(albumType);
				await db.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<AlbumType>> GetPageByBandAsync(int pageNumber, int pageSize, int bandId)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.AlbumTypes.Where(p => p.BandId == bandId)
										  .OrderByDescending(p => p.Name)
										  .Skip(skip)
										  .Take(pageSize)
										  .ToArrayAsync();
			}
		}
		public async Task<IEnumerable<AlbumType>> GetPageByBandAsync(int pageNumber, int pageSize, string bandName)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.AlbumTypes.Where(p => p.Band.Name == bandName)
										  .OrderByDescending(p => p.Name)
										  .Skip(skip)
										  .Take(pageSize)
										  .ToArrayAsync();
			}
		}

		public async Task<IEnumerable<AlbumType>> GetPageAsync(int pageNumber, int pageSize)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.AlbumTypes
										.OrderByDescending(p => p.Name)
										.Skip(skip)
										.Take(pageSize)
										.ToArrayAsync();
			}
		}

		//public async Task<IEnumerable<AlbumType>> GetAlbumTypesByAuthorAsync(string authorId)
		//{
		//	using (var db = new MusicUniEntities())
		//	{
		//		return await db.AlbumTypes.Include("AspNetUser")
		//								.Where(p => p.AuthorId == authorId)
		//								.OrderByDescending(post => post.Published)
		//								.ToArrayAsync();
		//	}
		//}
	}
}
