using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace TilePuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ImageBox[][] _images = new ImageBox[5][];
        private BitmapImage _stock;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InitGame()
        {
            if (_stock == null) return;
            Board.Children.Clear();
            for (var i = 0; i < 5; i++)
            {
                _images[i] = new ImageBox[5];

                for (var j = 0; j < 5; j++)
                {
                    var piece = new Image();
                    CroppedBitmap crop;
                    if (i != 4 || j != 4)
                    {
                        crop = new CroppedBitmap(_stock, new Int32Rect(i*100, j*100, 100, 100));
                    }
                    else
                    {
                        crop = null;
                    }
                    piece.Source = crop;
                    piece.MouseDown += TileClicked;
                    _images[i][j] = new ImageBox
                    {
                        Image = piece,
                        X = i,
                        Y = j
                    };

                    Board.Children.Add(_images[i][j].Image);
                    Canvas.SetLeft(_images[i][j].Image, i*100);
                    Canvas.SetTop(_images[i][j].Image, j*100);
                }
            }
        }

        private void TileClicked(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            ImageBox image = null;
            foreach (var imageBox in _images)
            {
                foreach (var box in imageBox.Where(box => box.Image.Equals(sender)))
                {
                    image = box;
                    break;
                }
            }
            if (image == null) return;
            if (image.X != 0 && _images[image.X-1][image.Y].Image.Source == null)
            {
                Swap(image, _images[image.X - 1][image.Y]);
            }
            else if (image.X != 4 && _images[image.X + 1][image.Y].Image.Source == null)
            {
                Swap(image, _images[image.X + 1][image.Y]);
            }
            else if (image.Y != 0 && _images[image.X][image.Y - 1].Image.Source == null)
            {
                Swap(image, _images[image.X][image.Y - 1]);
            }
            else if (image.Y != 4 && _images[image.X][image.Y + 1].Image.Source == null)
            {
                Swap(image, _images[image.X][image.Y + 1]);
            }
        }

        private static void Swap(ImageBox i, ImageBox j)
        {
            var temp = i.Image.Source;
            i.Image.Source = j.Image.Source;
            j.Image.Source = temp;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (dialog.ShowDialog() != true) return;
            _stock = new BitmapImage(new Uri(dialog.FileName));
            InitGame();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            InitGame();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (_stock == null) return;
            var rand = new Random();
            for (var i = 0; i < 100; i++)
            {
                foreach (var box in from imageBox in _images from box in imageBox where box.Image.Source == null select box)
                {
                    int x, y;
                    do
                    {
                        x = rand.Next(-1, 2) + box.X;
                        y = rand.Next(-1, 2) + box.Y;
                    } while (!(x >= 0 && x <= 4 && y >= 0 && y <= 4));
                    Swap(box, _images[x][y]);
                }
            }
        }
    }
    class ImageBox
    {
        public Image Image;
        public int X;
        public int Y;
    }
}
