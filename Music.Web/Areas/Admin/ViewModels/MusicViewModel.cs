using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Music.Core.Entities;

namespace Music.Web.Areas.Admin.ViewModels
{
	public class MusicViewModel
	{
		public string GenreOldName { get; set; }
		public string GenreNewName { get; set; }
		public IEnumerable<Genre> Genres { get; set; }

		public string BandOldName { get; set; }
		public string BandNewName { get; set; }
		public string BandDescption { get; set; }
		public string BandImage { get; set; }
		public IEnumerable<Band> Bands { get; set; }

		public string AlbumTypeOldName { get; set; }
		public string AlbumTypeNewName { get; set; }
		public string AlbumTypeDescption { get; set; }
		public string AlbumTypeImage { get; set; }
		public IEnumerable<AlbumType> AlbumTypes { get; set; }

		public string AlbumOldName { get; set; }
		public string AlbumNewName { get; set; }
		public string AlbumDescption { get; set; }
		public string AlbumImage { get; set; }
		public IEnumerable<Album> Albums { get; set; }

		public string FileOldName { get; set; }
		public string FileNewName { get; set; }
		public string FileRoot { get; set; }
		public int FileSize { get; set; }
		public string FileType { get; set; }
		public string FileDescption { get; set; }
		public string FileImage { get; set; }
		public IEnumerable<File> Files { get; set; }

		public string ActionType { get; set; }
	}
}