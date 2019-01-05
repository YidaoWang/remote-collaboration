using RemoteCollaboration.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace RemoteCollaboration.View.Controls
{
    /// <summary>
    /// Piece.xaml の相互作用ロジック
    /// </summary>
    public partial class Piece : Thumb
    {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(Piece));

        /// <summary>
        /// パズルピースのコンストラクタ
        /// </summary>
        /// <param name="i">行番号</param>
        /// <param name="j">列番号</param>
        /// <param name="n">行数</param>
        /// <param name="path">画像URI</param>
        /// <param name="puzzle">他ピースへの参照</param>
        public Piece(int i, int j, int n, Uri path, Piece[,] puzzle)
        {
            InitializeComponent();
            CombiningPieces = new List<Piece>();
            CombiningPieces.Add(this);
            I = i;
            J = j;
            N = n;
            var image = new BitmapImage(path);
            var width = (int)image.Width / n;
            var height = (int)image.Height / n;
            var rect = new Int32Rect(i * width, j * height, width, height);
            ImageSource = new CroppedBitmap(image, rect);
            _puzzle = puzzle;
        }

        /// <summary>
        /// 画像ソース
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// 結合しているピース（自身を含む）
        /// </summary>
        public List<Piece> CombiningPieces { get; set; }

        /// <summary>
        /// 行番号
        /// </summary>
        public int I { get; private set; }

        /// <summary>
        /// 列番号
        /// </summary>
        public int J { get; private set; }

        /// <summary>
        /// 行数
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// 結合
        /// </summary>
        /// <returns></returns>
        public void Combin(Piece piece)
        {
            if (!CombiningPieces.Contains(piece))
            {
                CombiningPieces.AddRange(piece.CombiningPieces);               
                foreach (var p in CombiningPieces)
                {
                    p.CombiningPieces = CombiningPieces;
                    var left = Canvas.GetLeft(this) + (p.I - I) * (Width + 1);
                    var top = Canvas.GetTop(this) + (p.J - J) * (Height + 1);
                    Canvas.SetLeft(p, left);
                    Canvas.SetTop(p, top);
                }
            }
        }

        /// <summary>
        /// 直近の場所に戻す
        /// </summary>
        public void ResetPlace()
        {
            Canvas.SetLeft(this, _lastLeft);
            Canvas.SetTop(this, _lastTop);
        }

        private Piece[,] _puzzle;
        private double _lastLeft;
        private double _lastTop;

        /// <summary>
        /// 結合ピース全体のドラッグ開始処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var tmp = CombiningPieces.ToArray();
            foreach (var piece in tmp)
            {
                piece.OnDragStarted();
            }
        }

        /// <summary>
        /// 個々のドラッグ開始処理
        /// </summary>
        private void OnDragStarted()
        {
            _lastLeft = Canvas.GetLeft(this);
            _lastTop = Canvas.GetTop(this);

            var border = Template.FindName("Thumb_Border", this) as Border;
            if (null != border)
            {
                border.BorderThickness = new Thickness(1);
            }
        }

        /// <summary>
        /// 結合ピース全体のドラッグ完了処理
        /// </summary>
        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var tmp = CombiningPieces.ToArray();
            var complete = true;
            foreach (var piece in tmp)
            {
                if (!piece.CanCompletDrag())
                {
                    complete = false;
                    break;
                }
            }
            if (complete)
            {
                foreach (var piece in tmp)
                {
                    piece.OnDragCompleted();
                }
            }
            else
            {
                foreach (var piece in tmp)
                {
                    piece.ResetPlace();
                }
            }
        }

        /// <summary>
        /// 個々のドラッグ完了可能判定
        /// </summary>
        private bool CanCompletDrag()
        {
            var x = Canvas.GetLeft(this);
            var y = Canvas.GetTop(this);

            // ドロップした際にピースが重ならないように
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i != I || j != J)
                    {
                        var piece = _puzzle[i, j];
                        var px = Canvas.GetLeft(piece);
                        var py = Canvas.GetTop(piece);
                        if (x < px + piece.Width && px + piece.Width < x + Width + piece.Width
                            && y < py + piece.Height && py + piece.Height < y + Height + piece.Height)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 個々のドラッグ完了処理
        /// </summary>
        private void OnDragCompleted()
        {
            var x = Canvas.GetLeft(this);
            var y = Canvas.GetTop(this);

            // ピースの結合
            // 左
            if (I > 0)
            {
                var piece = _puzzle[I - 1, J];
                var px = Canvas.GetLeft(piece);
                var py = Canvas.GetTop(piece);
                if (CanCombin(x, y, px + piece.Width, py))
                {
                    Combin(piece);
                }
            }
            // 右
            if (I < N - 1)
            {
                var piece = _puzzle[I + 1, J];
                var px = Canvas.GetLeft(piece);
                var py = Canvas.GetTop(piece);
                if (CanCombin(x + Width, y, px, py))
                {
                    Combin(piece);
                }
            }
            // 上
            if (J > 0)
            {
                var piece = _puzzle[I, J - 1];
                var px = Canvas.GetLeft(piece);
                var py = Canvas.GetTop(piece);
                if (CanCombin(x, y, px, py + piece.Height))
                {
                    Combin(piece);
                }
            }
            // 下
            if (J < N - 1)
            {
                var piece = _puzzle[I, J + 1];
                var px = Canvas.GetLeft(piece);
                var py = Canvas.GetTop(piece);
                if (CanCombin(x, y + Height, px, py))
                {
                    Combin(piece);
                }
            }

            var border = Template.FindName("Thumb_Border", this) as Border;
            if (null != border)
            {
                border.BorderThickness = new Thickness(0);
            }
        }

        /// <summary>
        /// 結合ピース全体のドラッグ中処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var tmp = CombiningPieces.ToArray();
            foreach (var piece in tmp)
            {
                piece.OnDragDelta(piece, e);
            }
        }

        /// <summary>
        /// 個々のドラッグ中処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (null != thumb)
            {
                var x = Canvas.GetLeft(thumb) + e.HorizontalChange;
                var y = Canvas.GetTop(thumb) + e.VerticalChange;

                var canvas = thumb.Parent as Canvas;
                if (null != canvas)
                {
                    x = Math.Max(x, 0);
                    y = Math.Max(y, 0);
                    x = Math.Min(x, canvas.ActualWidth - thumb.ActualWidth);
                    y = Math.Min(y, canvas.ActualHeight - thumb.ActualHeight);
                }

                Canvas.SetLeft(thumb, x);
                Canvas.SetTop(thumb, y);
            }
        }

        /// <summary>
        /// 結合可能な距離判定
        /// </summary>
        private bool CanCombin(double x, double y, double px, double py)
        {
            var combPix = 5;
            return Math.Abs(x - px) <= combPix && Math.Abs(y - py) <= combPix;
        }
    }
}
