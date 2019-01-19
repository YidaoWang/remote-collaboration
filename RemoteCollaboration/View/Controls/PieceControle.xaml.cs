using RemoteCollaboration.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RemoteCollaboration.View.Controls
{
    /// <summary>
    /// Piece.xaml の相互作用ロジック
    /// </summary>
    public partial class PieceControle : Thumb
    {
        public PieceViewModel ViewModel
        {
            get
            {
                return DataContext as PieceViewModel;
            }
        }
        
        public PieceControle()
        {
            InitializeComponent();
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            ViewModel.DragStarted();
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ViewModel.DragCompleted();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var canvas = Parent as Canvas;
            ViewModel.DragDelta(e.HorizontalChange, e.VerticalChange, canvas.ActualWidth, canvas.ActualHeight);
        }
    }
}
