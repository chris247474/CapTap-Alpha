using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Xamarin.Forms;
using Capp2.iOS.Helpers;
using System.IO;

[assembly: Dependency(typeof(SQLite_iOS))]
namespace Capp2.iOS.Helpers
{
public class SQLite_iOS : ISQLite
    {
        public SQLite_iOS() { }
        public SQLite.SQLiteConnection GetConnection()
        {
            /*var sqliteFilename = "CAPPDB26.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = System.IO.Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = System.IO.Path.Combine(libraryPath, sqliteFilename);
            // Create the connection
            var conn = new SQLite.SQLiteConnection(path);
            // Return the database connection
            return conn;*/

            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = System.IO.Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = System.IO.Path.Combine(documentsPath, "CAPPDB26.db3");

            if (!File.Exists(path))
            {
                App.firstRun = true;
                Console.WriteLine("Database doesn't exist yet, copying one-----------------------------------------------------------------------");
                /*if (!File.Exists(path))
                {
                    using (var br = new BinaryReader(Forms.Context.Resources.OpenRawResource(Resource.Raw.CAPPDB26)))
                    {
                        using (var bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
                        {
                            byte[] buffer = new byte[2048];
                            int length = 0;
                            while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                bw.Write(buffer, 0, length);
                            }
                        }
                    }
                }*/
            }

            var conn = new SQLite.SQLiteConnection(path);

            // Return the database connection 
            return conn;
        }

        public SQLiteConnection GetConnectionCAPP()
        {
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, "CAPPDB26.db3");

            if (!File.Exists(path))
            {
                App.firstRun = true;
                Console.WriteLine("CAPP Database doesn't exist yet, copying one-----------------------------------------------------------------------");
               /* if (!File.Exists(path))
                {
                    using (var br = new BinaryReader(Forms.Context.Resources.OpenRawResource(Resource.Raw.CAPPDB26)))
                    {
                        using (var bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
                        {
                            byte[] buffer = new byte[2048];
                            int length = 0;
                            while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                bw.Write(buffer, 0, length);
                            }
                        }
                    }
                }*/
            }

            var conn = new SQLite.SQLiteConnection(path);

            // Return the database connection 
            return conn;
        }

        public SQLiteConnection GetConnectionPlaylists()
        {
            Console.WriteLine("entered GetConnectionPlaylists()--------------------------------------------------------------------");
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, "Playlist4.db3");

            Console.WriteLine("paths found -------------------------------------------------------------------------------------");
            // This is where we copy in the prepopulated database
            Console.WriteLine(path);
            if (!File.Exists(path))
            {
                App.firstRun = true;
                Console.WriteLine("Playlist Database doesn't exist yet, copying one-----------------------------------------------------------------------");
                /* if (!File.Exists(path))
                 {
                     using (var br = new BinaryReader(Forms.Context.Resources.OpenRawResource(Resource.Raw.Playlist4)))
                     {
                         using (var bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
                         {
                             byte[] buffer = new byte[2048];
                             int length = 0;
                             while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                             {
                                 bw.Write(buffer, 0, length);
                             }
                         }
                     }
                 }*/
            }

            var conn = new SQLite.SQLiteConnection(path);

            // Return the database connection 
            return conn;
        }
    }
}
