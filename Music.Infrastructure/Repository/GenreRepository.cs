using Music.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Music.Core.Interfaces;

namespace Music.Infrastructure.Repository
{
	public class GenreRepository : IGenreRepository
	{
		public async Task<Genre> GetByIdAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var genre = await db.Genres.SingleOrDefaultAsync(p => p.Id == id);

				if (genre == null)
				{
					throw new KeyNotFoundException("The Genre Id \"" + id + "\" does not exist.");
				}

				return genre;
			}
		}
		public async Task<Genre> GetByNameAsync(string name)
		{
			using (var db = new MusicUniEntities())
			{
				var genre = await db.Genres.SingleOrDefaultAsync(p => p.Name == name);

				if (genre == null)
				{
					throw new KeyNotFoundException("The Genre name \"" + name + "\" does not exist.");
				}

				return genre;
			}
		}
		public async Task<IEnumerable<Genre>> GetAllyAsync()
		{
			using (var db = new MusicUniEntities())
			{
				var genre = await db.Genres.OrderBy(g => g.Name)
											   .ToListAsync();

				return genre;
			}
		}
		public async Task CreateAsync(Genre model)
		{
			using (var db = new MusicUniEntities())
			{
				var genre =
					await db.Genres.SingleOrDefaultAsync(pro => pro.Name == model.Name);

				if (genre != null)
				{
					throw new ArgumentException("A genre with the id of " + model.Id + " already exsits.");
				}

				db.Genres.Add(model);
				await db.SaveChangesAsync();
			}
		}

		public async Task EditAsync(int id, Genre updateItem)
		{
			using (var db = new MusicUniEntities())
			{
				var genre = await db.Genres.SingleOrDefaultAsync(p => p.Id == id);

				if (genre == null)
				{
					throw new KeyNotFoundException("A genre withthe id of " + id + "does not exisst in the data store");
				}

				genre.Name = updateItem.Name;

				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(int id)
		{
			using (var db = new MusicUniEntities())
			{
				var genre = await db.Genres.SingleOrDefaultAsync(p => p.Id == id);

				if (genre == null)
				{
					throw new KeyNotFoundException("The genre with the id of " + id + "does not exist.");
				}

				db.Genres.Remove(genre);
				await db.SaveChangesAsync();
			}
		}
		//public async Task<IEnumerable<Genre>> GetPageByCategoryAsync(int pageNumber, int pageSize, string category)
		//{
		//	using (var db = new MusicUniEntities())
		//	{
		//		var skip = (pageNumber - 1) * pageSize;

		//		return await db.Genres.Where(p => p.Published < DateTime.Now && p.Category.Name == category)
		//			.Include("AspNetUser")
		//			.OrderByDescending(p => p.Published)
		//			.Skip(skip)
		//			.Take(pageSize)
		//			.ToArrayAsync();
		//	}
		//}

		public async Task<IEnumerable<Genre>> GetPageAsync(int pageNumber, int pageSize)
		{
			using (var db = new MusicUniEntities())
			{
				var skip = (pageNumber - 1) * pageSize;

				return await db.Genres
										.OrderByDescending(p => p.Name)
										.Skip(skip)
										.Take(pageSize)
										.ToArrayAsync();
			}
		}

		//public async Task<IEnumerable<Genre>> GetGenresByAuthorAsync(string authorId)
		//{
		//	using (var db = new MusicUniEntities())
		//	{
		//		return await db.Genres.Include("AspNetUser")
		//								.Where(p => p.AuthorId == authorId)
		//								.OrderByDescending(post => post.Published)
		//								.ToArrayAsync();
		//	}
		//}
	}
}
