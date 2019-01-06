using RemoteCollaboration.Model;
using RemoteCollaboration.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace RemoteCollaboration.ViewModel
{
    public class CollaborationViewModel : ViewModelBase
    {
        #region Binding Properties
        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }

        public Puzzle Puzzle { get; set; }
        #endregion

        public CollaborationViewModel(NavigationService navigation) : base(navigation)
        {
            Score = 0;
            Time = 0;
            CanvasWidth = 1280;
            CanvasHeight = 650;
            Puzzle = new Puzzle(new Uri("Images/01.jpeg", UriKind.Relative), 400, 4, 5);
            Puzzle.RandomizePositions(CanvasWidth, CanvasHeight);
        }
    }
}
