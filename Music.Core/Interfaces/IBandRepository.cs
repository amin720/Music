using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.Core.Entities;

namespace Music.Core.Interfaces
{
	public interface IBandRepository
	{
		Task<Band> GetByIdAsync(int id);
		Task<Band> GetByNameAsync(string name);
		Task<IEnumerable<Band>> GetAllyAsync();
		Task CreateAsync(Band model);
		Task EditAsync(int id, Band updateItem);
		Task DeleteAsync(int id);
		Task SetBandToGenre(string bandName, string genreName);
		Task<IEnumerable<Band>> GetPageByGenreAsync(int pageNumber, int pageSize, string genreName);
		Task<IEnumerable<Band>> GetPageAsync(int pageNumber, int pageSize);
	}
}
