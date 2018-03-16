using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.Core.Entities;

namespace Music.Core.Interfaces
{
	public interface IGenreRepository
	{
		Task<Genre> GetByIdAsync(int id);
		Task<Genre> GetByNameAsync(string name);
		Task<IEnumerable<Genre>> GetAllyAsync();
		Task CreateAsync(Genre model);
		Task EditAsync(int id, Genre updateItem);
		Task DeleteAsync(int id);
		Task<IEnumerable<Genre>> GetPageAsync(int pageNumber, int pageSize);
	}
}
