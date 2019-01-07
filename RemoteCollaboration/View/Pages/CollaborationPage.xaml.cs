using RemoteCollaboration.Model;
using RemoteCollaboration.View.Controls;
using RemoteCollaboration.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace RemoteCollaboration.View.Pages
{
    /// <summary>
    /// CollaborationPage.xaml の相互作用ロジック
    /// </summary>
    public partial class CollaborationPage : Page
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        private CollaborationViewModel ViewModel
        {
            get
            {
                return DataContext as CollaborationViewModel;
            }
        }

        public CollaborationPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ページ読み込みイベント
        /// ViewModel のデータからピースを生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Puzzle puzzle = ViewModel.Puzzle;
            int n = ViewModel.Puzzle.N;
            var pieces = new Piece[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var piece = new Piece(i, j, n, puzzle.ImageUri, pieces);
                    piece.Moved += Piece_Moved;
                    piece.Combined += Piece_Combined;
                    pieces[i, j] = piece;
                    MainCanvas.Children.Add(piece);
                    Canvas.SetLeft(piece, puzzle.Positions[i, j].X);
                    Canvas.SetTop(piece, puzzle.Positions[i, j].Y);
                }
            }
        }

        /// <summary>
        /// ピースの動きを ViewModel に反映
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void Piece_Moved(Piece sender, double x, double y)
        {
            ViewModel.Puzzle.Positions[sender.I, sender.J] = new Point(x, y);
        }

        /// <summary>
        /// ピースの結合を ViewModel に反映
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="combine"></param>
        private void Piece_Combined(Piece sender, Piece combinePiece)
        {
            if (combinePiece != null)
            {
                ViewModel.Combined();
                if(combinePiece.CombiningPieces.Count == sender.N * sender.N)
                {
                    ViewModel.Finish();
                }
            }
            else
            {
                ViewModel.MissCombined();
            }
        }
    }
}