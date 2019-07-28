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
            MainGameContentControl.Content = new PlayAreaUserControl();
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
                _timer.Start();
            }
        }

        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = ".dat";
            dialog.CheckPathExists = true;

            if (dialog.ShowDialog() == true)
            {
                var dataSrc = MainGameContentControl.Content as ISaveable;
                SaveData data = new SaveData();
                data.bitmapImage = dataSrc.Image;
                data.location = dataSrc.Positions;
                _dao.Save(data, dialog.FileName);
            }
        }

        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == true)
            {
                var data = _dao.Load(dialog.FileName);
                // TODO: Load new UserControl
            }
        }
    }
}
