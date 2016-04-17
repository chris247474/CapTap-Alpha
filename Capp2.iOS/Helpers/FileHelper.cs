using Foundation;
using System.IO;

namespace Capp2.Helpers
{
    public static class FileHelper
    {
        public static string GetLocalFilePath(string filename, string dbName)
        {
            string docFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            string dbPath = Path.Combine(libFolder, filename);

            CopyDatabaseIfNotExists(dbPath, dbName);

            return dbPath;
        }

        private static void CopyDatabaseIfNotExists(string dbPath, string dbName)
        {
            if (!File.Exists(dbPath))
            {
                var existingDb = NSBundle.MainBundle.PathForResource(dbName, "db3");
                File.Copy(existingDb, dbPath);
            }
        }
    }
}
