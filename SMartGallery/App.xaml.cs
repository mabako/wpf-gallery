using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SMartGallery
{
    /// <summary>
    /// Logic for "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            /*
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                Application.Current.Shutdown();
                return;
            }

            string path = dialog.SelectedPath;
            */

            // Verzeichnispfad
            string path = "S:";

            // Datenbank relativ zum Verzeichnis anlegen/laden
            Database database = new Database(path + "\\.smart");

            // Sämtliche Verzeichnisse in neuem Thread durchlaufen
            new Thread(new DirectoryScanner(path, database).scan).Start();
        }
    }
}
