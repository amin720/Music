using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.Core.Entities;

namespace Music.Core.Interfaces
{
	public interface IAlbumTypeRepository
	{
		Task<AlbumType> GetByIdAsync(int id);
		Task<AlbumType> GetByNameAsync(string name);
		Task<AlbumType> GetByBandAsync(int bandId, string albumTypeName);
		Task<IEnumerable<AlbumType>> GetAllyAsync();
		Task CreateAsync(AlbumType model);
		Task EditAsync(int id, AlbumType updateItem);
		Task DeleteAsync(int id);
		Task<IEnumerable<AlbumType>> GetPageByBandAsync(int pageNumber, int pageSize, int bandId);
		Task<IEnumerable<AlbumType>> GetPageByBandAsync(int pageNumber, int pageSize, string bandName);
		Task<IEnumerable<AlbumType>> GetPageAsync(int pageNumber, int pageSize);
	}
}
