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
	public class BandRepository : IBandRepository
	{
		public async Task<Band> GetByIdAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var band = await db.Bands.SingleOrDefaultAsync(p => p.Id == id);

				//if (band == null)
				//{
				//	throw new KeyNotFoundException("The Band Id \"" + id + "\" does not exist.");
				//}

				return band;
			}
		}
		public async Task<Band> GetByNameAsync(string name)
		{
			using (var db = new MusicUniEntities())
			{
				var band = await db.Bands.SingleOrDefaultAsync(p => p.Name == name);

				//if (band == null)
				//{
				//	throw new KeyNotFoundException("The Band name \"" + name + "\" does not exist.");
				//}

				return band;
			}
		}
		public async Task<IEnumerable<Band>> GetAllyAsync()
		{
			using (var db = new MusicUniEntities())
			{
				var band = await db.Bands.OrderBy(g => g.Name)
											   .ToListAsync();

				return band;
			}
		}
		public async Task CreateAsync(Band model)
		{
			using (var db = new MusicUniEntities())
			{
				var band =
					await db.Bands.SingleOrDefaultAsync(pro => pro.Name == model.Name);

				if (band != null)
				{
					throw new ArgumentException("A band with the id of " + model.Id + " already exsits.");
				}
				
				db.Bands.Add(model);
				await db.SaveChangesAsync();
			}
		}

		public async Task EditAsync(int id, Band updateItem)
		{
			using (var db = new MusicUniEntities())
			{
				var band = await db.Bands.SingleOrDefaultAsync(p => p.Id == id);

				if (band == null)
				{
					throw new KeyNotFoundException("A band withthe id of " + id + "does not exisst in the data store");
				}

				band.Name = updateItem.Name;
				band.Description = updateItem.Description;
				band.ImageUrl = updateItem.ImageUrl;

				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var band = await db.Bands.SingleOrDefaultAsync(p => p.Id == id);

				if (band == null)
				{
					throw new KeyNotFoundException("The band with the id of " + id + "does not exist.");
				}

				db.Bands.Remove(band);
				await db.SaveChangesAsync();
			}
		}

		public async Task SetBandToGenre(string bandName, string genreName)
		{
			using (var db = new MusicUniEntities())
			{
				var band = await db.Bands.SingleOrDefaultAsync(p => p.Name == bandName);
				var genre = await db.Genres.SingleOrDefaultAsync(p => p.Name == genreName);

				if (band == null)
				{
					throw new KeyNotFoundException("The band with the Name of " + bandName + "does not exist.");
				}
				if (genre == null)
				{
					throw new KeyNotFoundException("The genre with the Name of " + bandName + "does not exist.");
				}

				genre.Bands.Add(band);

				await db.SaveChangesAsync();
			}
		}
		public async Task<IEnumerable<Band>> GetPageByGenreAsync(int pageNumber, int pageSize, string genreName)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.Bands.Where(p => p.Genres.Any(g => g.Name == genreName)  )
									 .OrderByDescending(p => p.Name)
									 .Skip(skip)
									 .Take(pageSize)
									 .ToArrayAsync();
			}
		}

		public async Task<IEnumerable<Band>> GetPageAsync(int pageNumber, int pageSize)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.Bands
										.OrderByDescending(p => p.Name)
										.Skip(skip)
										.Take(pageSize)
										.ToArrayAsync();
			}
		}

		//public async Task<IEnumerable<Band>> GetBandsByAuthorAsync(string authorId)
		//{
		//	using (var db = new MusicUniEntities())
		//	{
		//		return await db.Bands.Include("AspNetUser")
		//								.Where(p => p.AuthorId == authorId)
		//								.OrderByDescending(post => post.Published)
		//								.ToArrayAsync();
		//	}
		//}
	}
}
