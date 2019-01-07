using System;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace RemoteCollaboration.Model
{
    public class Experiment
    {
        public int Score { get; set; }
        public int Time { get; set; }

        public Experiment()
        {
            Score = 0;
            Time = 0;
        }

        public void Start()
        { 
            var now = DateTime.Now;

            Observable.Interval(TimeSpan.FromMilliseconds(10), Scheduler.Default)
                    .Subscribe(time => {
                        Time = (DateTime.Now - now).Seconds;
                    });
        }

        public void Combined()
        {
            Score += 10;
        }

        public void MissCombined()
        {
            Score -= 10;
        }
    }
}
