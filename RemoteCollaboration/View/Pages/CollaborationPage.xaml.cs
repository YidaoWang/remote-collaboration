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
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var piece = new PieceControle
                    {
                        DataContext = new PieceViewModel(NavigationService, puzzle.Pieces[i, j])
                    };
                    MainCanvas.Children.Add(piece);
                }
            }
        }
    }
}
