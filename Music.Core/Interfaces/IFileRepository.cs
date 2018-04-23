using Music.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Music.Core.Interfaces
{
	public interface IFileRepository
	{
		Task<File> GetByIdAsync(int id);
		Task<File> GetByNameAsync(string fileName);
		Task<File> GetByAlbumAsync(int albumId, string fileName);
		Task<IEnumerable<File>> GetAllyAsync();
		Task CreateAsync(File model);
		Task EditAsync(int id, File updateItem);
		Task DeleteAsync(int id);
		Task<IEnumerable<File>> GetPageByAlbumAsync(int pageNumber, int pageSize, int albumId);
		Task<IEnumerable<File>> GetPageByAlbumAsync(int pageNumber, int pageSize, string albumName);
		Task<IEnumerable<File>> GetPageAsync(int pageNumber, int pageSize);
	}
}
