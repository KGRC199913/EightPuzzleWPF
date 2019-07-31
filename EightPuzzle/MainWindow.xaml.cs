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
        const int TIME_LIMIT = 180;
        GameTimer _timer = new GameTimer(TIME_LIMIT);
        IDao _dao = new SaveLoadManager();

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

        private void OnGameVictory()
        {
            MessageBox.Show("Meow, u won");
        }

        private void QuitGameButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
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
                MainGameContentControl.Content = new PlayAreaUserControl(path);
                (MainGameContentControl.Content as PlayAreaUserControl).OnVictory += OnGameVictory;
                _timer.Start();
            }
        }

        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Pause();
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = ".dat";
            dialog.CheckPathExists = true;

            if (dialog.ShowDialog() == true)
            {
                var dataSrc = MainGameContentControl.Content as ISaveable;
                SaveData data = new SaveData();
                data.bitmapImage = FullImage.Source as BitmapImage;
                data.location = dataSrc.Positions;
                data.time = _timer.Second;
                _dao.Save(data, dialog.FileName);
            }
            _timer.Resume();
        }

        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Pause();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                var data = _dao.Load(dialog.FileName);
                if (data == null)
                    return;
                FullImage.Source = data.bitmapImage;
                FullImage.Visibility = Visibility.Visible;
                LoadImageButton.Visibility = Visibility.Collapsed;
                _timer.Stop();
                _timer.Resume();
                _timer = new GameTimer(data.time);
                TimerLabel.DataContext = _timer;
                _timer.Start();
                MainGameContentControl.Content = new PlayAreaUserControl(FullImage.Source as BitmapImage, data.location);
                (MainGameContentControl.Content as PlayAreaUserControl).OnVictory += OnGameVictory;
            }
            else
                _timer.Resume();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            (MainGameContentControl.Content as PlayAreaUserControl).Window_KeyDown(sender, e);
            e.Handled = true;
        }

        private void MainGameGrid_Loaded(object sender, RoutedEventArgs e)
        {
            MainGameGrid.Focus();
        }

        private void RestartGameButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _timer = new GameTimer(TIME_LIMIT);
            TimerLabel.DataContext = _timer;
            LoadImageButton.Visibility = Visibility.Visible;
            FullImage.Visibility = Visibility.Collapsed;
        }
    }
}
