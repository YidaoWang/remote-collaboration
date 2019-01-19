using System;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.ComponentModel;

namespace RemoteCollaboration.Model
{
    public class Experiment : INotifyPropertyChanged
    {
        public int Score { get; set; }
        public int Time { get; set; }

        private IDisposable _disposable;

        public Experiment()
        {
            Score = 0;
            Time = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Start()
        {
            var now = DateTime.Now;

            _disposable = Observable.Interval(TimeSpan.FromMilliseconds(10), Scheduler.Default)
                    .Subscribe(time =>
                    {
                        Time = (int)(DateTime.Now - now).TotalSeconds;
                        RaisePropertyChanged(nameof(Time));
                    });
        }

        public void Stop()
        {
            _disposable.Dispose();
        }

        public void Combined()
        {
            Score += 10;
            RaisePropertyChanged(nameof(Score));
        }

        public void MissCombined()
        {
            Score -= 10;
            RaisePropertyChanged(nameof(Score));
        }
    }
}
