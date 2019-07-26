using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EightPuzzle
{
    public delegate void InvokeOnStop();

    class GameTimer : INotifyPropertyChanged
    {
        private int _second;
        private bool _isPaused;
        private BackgroundWorker _worker;

        public GameTimer(int second)
        {
            _second = second;
            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(
                (sender, e) =>
                {
                    Countdown();
                });
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                (sender, e) =>
                {
                    OnStop?.Invoke();
                });
        }

        public int Second {
            get => _second;
            set
            {
                _second = value;
                OnPropertyChanged("Second");
            }
        }

        public void Start()
        {
            _isPaused = false;
            _worker.RunWorkerAsync();
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        public event InvokeOnStop OnStop;
        public event PropertyChangedEventHandler PropertyChanged;

        private void Countdown()
        {
            while (_second != 0)
            {
                if (!_isPaused)
                {
                    Second -= 1;
                    Thread.Sleep(1000);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
