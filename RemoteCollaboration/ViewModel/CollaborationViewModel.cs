using RemoteCollaboration.Model;
using RemoteCollaboration.Util;
using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RemoteCollaboration.ViewModel
{
    public class CollaborationViewModel : ViewModelBase
    {
        public Puzzle Puzzle { get; set; }
        public Experiment Experiment { get; set; }
        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }
        public ICommand BackCommand { get; set; }

        public CollaborationViewModel(NavigationService navigation, Experiment exp) : base(navigation)
        {
            CanvasWidth = 1280;
            CanvasHeight = 650;
            BackCommand = new DelegateCommand(Back);
            Puzzle = new Puzzle(new Uri("Images/01.jpeg", UriKind.Relative), 400, 4, 5);
            Puzzle.Combined += Combined;
            Puzzle.RandomizePositions(CanvasWidth, CanvasHeight);
            Experiment = exp;
            Experiment.PropertyChanged += Experiment_PropertyChanged;
            Experiment.Start();
        }

        private void Combined(Piece sender, Piece combine)
        {
            if (combine != null)
            {
                Experiment.Combined();
            }
            else
            {
                Experiment.MissCombined();
            }
        }

        public void Finish()
        {
            Experiment.Stop();
            Navigate("Startup", new StartupViewModel(NavigationService, Experiment));
        }

        private void Back(object paramater)
        {
            Navigate("Startup", new StartupViewModel(NavigationService));
        }

        private void Experiment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Experiment));
        }
    }
}
