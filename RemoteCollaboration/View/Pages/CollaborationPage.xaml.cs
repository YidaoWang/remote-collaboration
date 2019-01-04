using RemoteCollaboration.Util;
using RemoteCollaboration.View.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteCollaboration.View.Pages
{
    /// <summary>
    /// CollaborationPage.xaml の相互作用ロジック
    /// </summary>
    public partial class CollaborationPage : Page
    {
        public CollaborationPage()
        {
            InitializeComponent();
            int n = 4;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    var thumb = new Piece(i, j, n, new Uri("Images/01.jpeg", UriKind.Relative));
                    MainCanvas.Children.Add(thumb);
                    Canvas.SetLeft(thumb, 100 + i * 101);
                    Canvas.SetTop(thumb, 100 + j * 101);
                }
        }

    }
}
