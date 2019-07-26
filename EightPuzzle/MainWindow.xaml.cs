using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EightPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameTimer _timer = new GameTimer(10);
        public MainWindow()
        {
            InitializeComponent();
            TimerLabel.DataContext = _timer;
            _timer.OnStop += new InvokeOnStop(
                () =>
                {
                    MessageBox.Show("Countdown finished");
                }
                );
        }

        private void QuitGameButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Pause();
        }

        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Resume();
        }

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                FullImage.Source = new BitmapImage(new Uri(path));
                LoadImageButton.Visibility = Visibility.Collapsed;
                FullImage.Visibility = Visibility.Visible;
                _timer.Start();
            }
        }
    }
}
