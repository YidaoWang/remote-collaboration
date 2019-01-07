using RemoteCollaboration.Model;
using RemoteCollaboration.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace RemoteCollaboration.ViewModel
{
    public class CollaborationViewModel : ViewModelBase
    {
        public Puzzle Puzzle { get; set; }
        public Experiment Experiment { get; set; }
        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }

        public CollaborationViewModel(NavigationService navigation) : base(navigation)
        {
            CanvasWidth = 1280;
            CanvasHeight = 650;
            Puzzle = new Puzzle(new Uri("Images/01.jpeg", UriKind.Relative), 400, 4, 5);
            Puzzle.RandomizePositions(CanvasWidth, CanvasHeight);

            Experiment = new Experiment();
            Observable.Interval(TimeSpan.FromMilliseconds(10), Scheduler.Default)
                  .Subscribe(time =>
                  {
                      RaisePropertyChanged(nameof(Experiment));
                  });
            Experiment.Start();
        }

        public void Combined()
        {
            Experiment.Combined();
            RaisePropertyChanged(nameof(Experiment));
        }

        public void MissCombined()
        {
            Experiment.MissCombined();
            RaisePropertyChanged(nameof(Experiment));
        }

        public void Finish()
        {
            Navigate(new Uri("View/Pages/StartupPage.xaml", UriKind.Relative), new StartupViewModel(NavigationService, Experiment));
        }
    }
}
