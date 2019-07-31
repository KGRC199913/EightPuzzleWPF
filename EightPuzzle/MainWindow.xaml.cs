using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public int TimeLimit
        {
            get => _timeLimit;
            set
            {
                _timeLimit = value;
                OnPropertyChanged("TimeLimit");
            }
        }
        int _timeLimit;
        GameTimer _timer = null;
        IDao _dao = new SaveLoadManager();
        bool keydownDisable = false;

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            PauseGameToggleButton.Visibility = Visibility.Collapsed;
            TimeSlider.DataContext = this;
            TimeLimit = 180; //Default time limit
        }

        private void OnGameOver()
        {
            if (_timer != null && _timer.Second <= 0)
            {
                MessageBox.Show("Time up", "Game Over", MessageBoxButton.OK, MessageBoxImage.Stop);
                MainGameContentControl.IsHitTestVisible = false;
                keydownDisable = true;
                PauseGameToggleButton.Visibility = Visibility.Collapsed;
            }
        }

        private void OnGameVictory()
        {
            _timer.Stop();
            MessageBox.Show("Congratulation, you have solved the puzzle", "Win", MessageBoxButton.OK, MessageBoxImage.None);
            keydownDisable = true;
            MainGameContentControl.IsHitTestVisible = false;
            PauseGameToggleButton.Visibility = Visibility.Collapsed;
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

                MainGameContentControl.IsHitTestVisible = true;
                MainGameContentControl.Content = new PlayAreaUserControl(path);
                (MainGameContentControl.Content as PlayAreaUserControl).OnVictory += OnGameVictory;

                keydownDisable = false;
                PauseGameToggleButton.Visibility = Visibility.Visible;
                PauseGameToggleButton.IsChecked = false;

                _timer = new GameTimer(_timeLimit);
                TimerLabel.DataContext = _timer;
                TimerLabel.Visibility = Visibility.Visible;
                _timer.OnStop += OnGameOver;

                TimeSlider.Visibility = Visibility.Collapsed;
                TimeChooseLabel.Visibility = Visibility.Collapsed;
                _timer.Start();
            }
        }

        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (FullImage.Source == null)
            {
                MessageBox.Show("Error: There is nothing to save", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
            _timer?.Pause();
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

                _timer?.Stop();
                _timer?.Resume();
                _timer = new GameTimer(data.time);
                _timer.OnStop += OnGameOver;
                TimerLabel.DataContext = _timer;
                TimerLabel.Visibility = Visibility.Visible;
                _timer.Start();

                MainGameContentControl.IsHitTestVisible = true;
                MainGameContentControl.Content = new PlayAreaUserControl(FullImage.Source as BitmapImage, data.location);
                (MainGameContentControl.Content as PlayAreaUserControl).OnVictory += OnGameVictory;

                keydownDisable = false;

                PauseGameToggleButton.Visibility = Visibility.Visible;
                PauseGameToggleButton.IsChecked = false;

                TimeSlider.Visibility = Visibility.Collapsed;
                TimeChooseLabel.Visibility = Visibility.Collapsed;
            }
            else
                _timer?.Resume();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!keydownDisable)
            {
                (MainGameContentControl.Content as PlayAreaUserControl).Window_KeyDown(sender, e);
            }
            e.Handled = true;
        }

        private void MainGameGrid_Loaded(object sender, RoutedEventArgs e)
        {
            MainGameGrid.Focus();
        }

        private void RestartGameButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _timer = null;

            TimerLabel.Visibility = Visibility.Collapsed;

            LoadImageButton.Visibility = Visibility.Visible;
            FullImage.Visibility = Visibility.Collapsed;

            FullImage.Source = null;
            MainGameContentControl.Content = null;
            MainGameContentControl.IsHitTestVisible = true;

            keydownDisable = false;
            PauseGameToggleButton.Visibility = Visibility.Collapsed;

            TimeSlider.Visibility = Visibility.Visible;
            TimeChooseLabel.Visibility = Visibility.Visible;
        }

        private void PauseGameToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            _timer?.Pause();
            MainGameContentControl.IsHitTestVisible = false;
            keydownDisable = true;
        }

        private void PauseGameToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            _timer?.Resume();
            MainGameContentControl.IsHitTestVisible = true;
            keydownDisable = false;
        }

        private void DarkModToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries[0].Source =
                new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml");
            Application.Current.Resources.MergedDictionaries[2].Source =
                new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Purple.xaml");
            Application.Current.Resources.MergedDictionaries[3].Source =
                new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.DeepPurple.xaml");
        }

        private void DarkModToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries[0].Source =
                new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml");
            Application.Current.Resources.MergedDictionaries[2].Source =
                new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.LightBlue.xaml");
            Application.Current.Resources.MergedDictionaries[3].Source =
                new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.Cyan.xaml");
        }
    }
}
