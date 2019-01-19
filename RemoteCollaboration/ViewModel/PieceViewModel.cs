using RemoteCollaboration.Model;
using RemoteCollaboration.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace RemoteCollaboration.ViewModel
{
    public class PieceViewModel : ViewModelBase
    {
        private Piece _piece;
        private Brush _borderBrush;
        private Thickness _borderThickness;

        public Brush BorderBrush
        {
            get => _borderBrush; set
            {
                _borderBrush = value;
                RaisePropertyChanged(nameof(BorderBrush));
            }
        }
        public Thickness BorderThickness
        {
            get => _borderThickness; set
            {
                _borderThickness = value;
                RaisePropertyChanged(nameof(BorderThickness));
            }
        }
        public double Left
        {
            get => _piece.Left; set
            {
                _piece.Left = value;
                RaisePropertyChanged(nameof(Left));
            }
        }
        public double Top
        {
            get => _piece.Top; set
            {
                _piece.Top = value;
                RaisePropertyChanged(nameof(Top));
            }
        }

        public int Width
        {
            get => _piece.Width; set
            {
                _piece.Width = value;
                RaisePropertyChanged(nameof(Width));
            }
        }

        public int Height
        {
            get => _piece.Height; set
            {
                _piece.Height = value;
                RaisePropertyChanged(nameof(Height));
            }
        }

        public ImageSource ImageSource
        {
            get => _piece.ImageSource;
        }


        public PieceViewModel(NavigationService navigation, Piece piece) : base(navigation)
        {
            _piece = piece;
            piece.Moved += Moved;
            piece.SelectionChanged += SelectionChanged;
        }

        public void DragStarted()
        {
            _piece.DragStarted();
        }

        public void DragCompleted()
        {
            _piece.DragCompleted();
        }

        public void DragDelta(double horizontalChange, double verticalChange, double canvasWidth, double canvasHeight)
        {
            _piece.DragDelta(horizontalChange, verticalChange, canvasWidth, canvasHeight);
        }

        private void Moved(Piece piece, double left, double top)
        {
            Left = left;
            Top = top;
        }

        private void SelectionChanged(Piece sender, Piece.EnumSelectedState selectedState)
        {
            switch (selectedState)
            {
                case Piece.EnumSelectedState.NotSelected:
                    BorderBrush = new SolidColorBrush(Colors.Red);
                    BorderThickness = new Thickness(0);
                    break;
                case Piece.EnumSelectedState.Selected:
                    BorderBrush = new SolidColorBrush(Colors.Red);
                    BorderThickness = new Thickness(1);
                    break;
                case Piece.EnumSelectedState.CollaboraterSelected:
                    BorderBrush = new SolidColorBrush(Colors.Blue);
                    BorderThickness = new Thickness(1);
                    break;
            }
        }
    }
}
