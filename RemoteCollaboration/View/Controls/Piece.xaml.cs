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
        /// 選択状態
        /// </summary>
        public enum EnumSelectedState
        {
            NotSelected,
            Selected,
            CollaboraterSelected,
        }
        public EnumSelectedState SelectedState { get; set; }

        /// <summary>
        /// 選択
        /// </summary>
        public void Select()
        {
            SelectedState = EnumSelectedState.Selected;
            var border = Template.FindName("Thumb_Border", this) as Border;
            if (null != border)
            {
                border.BorderThickness = new Thickness(1);
            }
        }

        /// <summary>
        /// コラボレーター選択
        /// </summary>
        public void CollaboraterSelect()
        {
            SelectedState = EnumSelectedState.CollaboraterSelected;
            var border = Template.FindName("Thumb_Border", this) as Border;
            if (null != border)
            {
                border.BorderThickness = new Thickness(1);
            }
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void SelectCancel()
        {
            SelectedState = EnumSelectedState.NotSelected;
            var border = Template.FindName("Thumb_Border", this) as Border;
            if (null != border)
            {
                border.BorderThickness = new Thickness(0);
            }
        }


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
                    p.MoveTo(left, top);
                }
            }
        }

        /// <summary>
        /// 直近の場所に戻す
        /// </summary>
        public void ResetPlace()
        {
            MoveTo(_lastLeft, _lastTop);
        }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="left"></param>
        public void MoveTo(double x, double y)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
            OnMove(this, x, y);
        }

        public delegate void MoveEventHander(Piece sender, double x, double y);
        /// <summary>
        /// ピース移動イベント
        /// </summary>
        public event MoveEventHander OnMove;

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
            switch (SelectedState)
            {
                case EnumSelectedState.NotSelected:
                    var isSelected = false;
                    var isColSelected = false;
                    Piece selected = null;
                    foreach (var piece in _puzzle)
                    {
                        if (piece.SelectedState == EnumSelectedState.Selected)
                        {
                            selected = piece;
                            isSelected = true;
                        }
                        if (piece.SelectedState == EnumSelectedState.CollaboraterSelected)
                        {
                            isColSelected = true;
                        }
                    }
                    if (!isColSelected)
                    {
                        foreach (var cmb in CombiningPieces)
                        {
                            cmb.CollaboraterSelect();
                        }
                    }
                    else if (isColSelected && !isSelected)
                    {
                        foreach (var cmb in CombiningPieces)
                        {
                            cmb.Select();
                        }
                    }
                    else
                    {
                        foreach (var selectedCmb in selected.CombiningPieces)
                        {
                            selectedCmb.SelectCancel();
                        }
                        foreach (var cmb in CombiningPieces)
                        {
                            cmb.Select();
                        }
                    }
                    break;
                case EnumSelectedState.Selected:
                    break;
                case EnumSelectedState.CollaboraterSelected:
                    foreach (var cmb in CombiningPieces)
                    {
                        cmb.SelectCancel();
                    }
                    break;
            }
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

            foreach (var csp in CollaboraterSelectedPieces)
            {
                var canCombin = false;
                switch (GetCombinEdge(this, csp))
                {
                    case 0:
                        canCombin = csp.I == I - 1 && csp.J == J;
                        break;
                    case 1:
                        canCombin = csp.I == I && csp.J == J - 1;
                        break;
                    case 2:
                        canCombin = csp.I == I + 1 && csp.J == J;
                        break;
                    case 3:
                        canCombin = csp.I == I && csp.J == J + 1;
                        break;
                    default:
                        continue;
                }
                if (canCombin)
                {
                    Combin(csp);
                    break;
                }
                else
                {
                    // ミス
                    break;
                }
            }
        }

        /// <summary>
        /// コラボレーターの選択しているピースを返す
        /// </summary>
        private List<Piece> CollaboraterSelectedPieces
        {
            get
            {
                foreach (var p in _puzzle)
                {
                    if (p.SelectedState == EnumSelectedState.CollaboraterSelected)
                    {
                        return p.CombiningPieces;
                    }
                }
                return new List<Piece>();
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
            var piece = sender as Piece;
            if (null != piece)
            {
                var x = Canvas.GetLeft(piece) + e.HorizontalChange;
                var y = Canvas.GetTop(piece) + e.VerticalChange;

                var canvas = piece.Parent as Canvas;
                if (null != canvas)
                {
                    x = Math.Max(x, 0);
                    y = Math.Max(y, 0);
                    x = Math.Min(x, canvas.ActualWidth - piece.ActualWidth);
                    y = Math.Min(y, canvas.ActualHeight - piece.ActualHeight);
                }
                piece.MoveTo(x, y);
            }
        }

        /// <summary>
        /// 結合距離判定
        /// </summary>
        private bool CanCombin(double x, double y, double px, double py)
        {
            var combPix = 10;
            return Math.Abs(x - px) <= combPix && Math.Abs(y - py) <= combPix;
        }

        /// <summary>
        /// 結合可能辺取得
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        /// -1: なし, 0: 左, 1:上, 2:右, 3:下
        /// </returns>
        private int GetCombinEdge(Piece a, Piece b)
        {
            if (CanCombin(Canvas.GetLeft(a), Canvas.GetTop(a), Canvas.GetLeft(b) + b.Width, Canvas.GetTop(b)))
            {
                return 0;
            }
            if (CanCombin(Canvas.GetLeft(a), Canvas.GetTop(a), Canvas.GetLeft(b), Canvas.GetTop(b) + b.Height))
            {
                return 1;
            }
            if (CanCombin(Canvas.GetLeft(a) + a.Width, Canvas.GetTop(a), Canvas.GetLeft(b), Canvas.GetTop(b)))
            {
                return 2;
            }
            if (CanCombin(Canvas.GetLeft(a), Canvas.GetTop(a) + a.Height, Canvas.GetLeft(b), Canvas.GetTop(b)))
            {
                return 3;
            }
            return -1;
        }
    }
}
