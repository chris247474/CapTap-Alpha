using System;
using SQLite;

namespace Capp2
{
	public interface ISQLite
	{
		SQLite.SQLiteConnection GetConnectionCAPP();
		SQLite.SQLiteConnection GetConnectionPlaylists();
	}
}

