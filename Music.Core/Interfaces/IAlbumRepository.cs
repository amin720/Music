using Music.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Music.Core.Interfaces
{
	public interface IAlbumRepository
	{
		Task<Album> GetByIdAsync(int id);
		Task<Album> GetByNameAsync(string name);
		Task<IEnumerable<Album>> GetAllyAsync();
		Task CreateAsync(Album model);
		Task EditAsync(int id, Album updateItem);
		Task DeleteAsync(int id);
		Task<IEnumerable<Album>> GetPageByAlbumTypeAsync(int pageNumber, int pageSize, int albumTypeId);
		Task<IEnumerable<Album>> GetPageByAlbumTypeAsync(int pageNumber, int pageSize, string albumTypeName);
		Task<IEnumerable<Album>> GetPageAsync(int pageNumber, int pageSize);
	}
}
