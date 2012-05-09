using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMartGallery
{
    /// <summary>
    /// Logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Path containing the picture displayed on the left.
        /// </summary>
        private string leftPath;

        /// <summary>
        /// Path containing the picture displayed on the right
        /// </summary>
        private string rightPath;

        /// <summary>
        /// Sides on which  a picture may be (left, right)
        /// </summary>
        private enum Side { LEFT, RIGHT };

        /// <summary>
        /// Constructor, initializes components.
        /// </summary>
        public MainWindow()
        {
            this.Loaded += OnLoaded;

            InitializeComponent();
        }

        /// <summary>
        /// Handle all keys accordingly
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnKeyDown(KeyEventArgs eventArgs)
        {
            if (eventArgs.Key == Key.Escape)
            {
                Close();
                DirectoryScanner.completedScan = true;
            }

            if (eventArgs.Key == Key.NumPad1)
            {
                Database.update(leftPath, "up");
                Database.update(rightPath, "down");

                newPictures();
            }

            if (eventArgs.Key == Key.NumPad2)
            {
                Database.update(leftPath, "up");
                Database.update(rightPath, "up");

                newPictures();
            }

            if (eventArgs.Key == Key.NumPad3)
            {
                Database.update(leftPath, "down");
                Database.update(rightPath, "up");

                newPictures();
            }

            if (eventArgs.Key == Key.NumPad5)
            {
                newPictures();
            }

            if (eventArgs.Key == Key.NumPad8)
            {
                Database.update(leftPath, "down");
                Database.update(rightPath, "down");

                newPictures();
            }
        }

        /// <summary>
        /// Handle all mouse interaction accordingly
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    Database.update(leftPath, "up");
                    Database.update(rightPath, "down");

                    newPictures();
                }

                if (e.ChangedButton == MouseButton.Right)
                {
                    Database.update(leftPath, "down");
                    Database.update(rightPath, "up");

                    newPictures();
                }

                if (e.ChangedButton == MouseButton.Middle)
                {
                    Database.update(leftPath, "up");
                    Database.update(rightPath, "up");

                    newPictures();
                }
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Scrolling 'down' with the mouse wheel loads new pictures
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if(e.Delta < 0)
                newPictures();
            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Load new pictures upon the window being loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            newPictures();
        }

        /// <summary>
        /// Loads new pictures for the left and right side.
        /// </summary>
        private void newPictures()
        {
            getNewPictureFor(Side.LEFT);
            getNewPictureFor(Side.RIGHT);
        }

        /// <summary>
        /// Loads a picture for the specified side.
        /// </summary>
        /// <param name="side">the side to load the picture for</param>
        private void getNewPictureFor(Side side)
        {
            string path = Database.getRandomPicture(leftPath, rightPath);
            if (path == null)
                return;

            try
            {
                BitmapImage image = new BitmapImage(new Uri(path));
                if (side == Side.LEFT)
                {
                    leftPath = path;
                    leftImage.Source = image;
                }
                else
                {
                    rightPath = path;
                    rightImage.Source = image;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
