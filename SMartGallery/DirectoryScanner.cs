using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMartGallery
{
    /// <summary>
    /// Iterates through all files in a directory recursively
    /// </summary>
    class DirectoryScanner
    {
        private string basePath;

        /// <summary>
        /// Referenced Database
        /// </summary>
        private Database database;

        public static bool completedScan;

        public DirectoryScanner(string basePath, Database database)
        {
            this.basePath = basePath;
            this.database = database;
        }

        /// <summary>
        /// Scans the basePath for any jpgs and pngs
        /// </summary>
        public void scan()
        {
            // Loop over basepath
            foreach(string path in GetFiles(basePath))
            {
                // Cancel if we're done already
                if (completedScan)
                    break;

                // Read the fileneme, if ends with .jpf or png
                FileInfo info = new FileInfo(path);
                if (info.Extension.ToLower().Equals(".jpg") || info.Extension.ToLower().Equals(".png"))
                {
                    database.insert(path);
                }
            }
        }

        /// <summary>
        /// Fetches all subdirectories and -files for the current path.
        /// </summary>
        /// <param name="path">parent path</param>
        /// <returns>*.jpg|*.png within the directory</returns>
        private IEnumerable<string> GetFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }
    }
}
