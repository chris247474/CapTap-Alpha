using System;
using SQLite;

namespace Capp2
{
	[Table ("Playlists")]
	public class Playlist
	{
		[PrimaryKey, AutoIncrement, Column("ID"), NotNull]
		public int ID { get; set; }

		[Column("PlaylistName"), NotNull, Unique]
		public string PlaylistName { get; set; }

		[Column("Icon")]
		public string Icon { get; set; } = "people.png";
	}
}

