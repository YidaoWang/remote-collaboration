using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RemoteCollaboration.Model
{
    public class Piece
    {
        private double _lastLeft;
        private double _lastTop;
        public Puzzle _puzzle;

        public Piece(Puzzle puzzle, int i, int j)
        {
            _puzzle = puzzle;
            CombiningPieces = new List<Piece>();
            CombiningPieces.Add(this);
            I = i;
            J = j;
            var image = new BitmapImage(puzzle.ImageUri);
            Width = (int)image.Width / puzzle.N;
            Height = (int)image.Height / puzzle.N;
            var rect = new Int32Rect(i * Width, j * Height, Width, Height);
            ImageSource = new CroppedBitmap(image, rect);
        }

        /// <summary>
        /// イメージソース
        /// </summary>
        public ImageSource ImageSource { get; set; }
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
        /// x座標
        /// </summary>
        public double Left { get; set; }
        /// <summary>
        /// y座標
        /// </summary>
        public double Top { get; set; }
        /// <summary>
        /// 高さ
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 幅
        /// </summary>
        public int Width { get; set; }
        

        /// <summary>
        /// ピース移動イベント
        /// </summary>
        public delegate void MoveEventHandler(Piece sender, double left, double top);
        public event MoveEventHandler Moved;

        /// <summary>
        /// 選択変化イベント
        /// </summary>
        public delegate void SelectionChangeEventHandler(Piece sender, EnumSelectedState selectedState);
        public event SelectionChangeEventHandler SelectionChanged;

        /// <summary>
        /// 結合イベント
        /// </summary>
        public delegate void CombineEventHandler(Piece sender, Piece combine);
        public event CombineEventHandler Combined;


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
            SelectionChanged(this, SelectedState);
        }

        /// <summary>
        /// コラボレーター選択
        /// </summary>
        public void CollaboraterSelect()
        {
            SelectedState = EnumSelectedState.CollaboraterSelected;
            SelectionChanged(this, SelectedState);
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void SelectCancel()
        {
            SelectedState = EnumSelectedState.NotSelected;
            SelectionChanged(this, SelectedState);
        }

        /// <summary>
        /// 移動
        /// </summary>
        public void MoveTo(double left, double top)
        {
            Left = left;
            Top = top;
            Moved(this, left, top);
        }

        /// <summary>
        /// 直近の場所に戻す
        /// </summary>
        public void ResetPlace()
        {
            MoveTo(_lastLeft, _lastTop);
        }

        /// <summary>
        /// 結合ピース全体のドラッグ開始処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DragStarted()
        {
            switch (SelectedState)
            {
                case EnumSelectedState.NotSelected:
                    var isSelected = false;
                    var isColSelected = false;
                    Piece selected = null;
                    foreach (var piece in _puzzle.Pieces)
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
                piece.EachDragStarted();
            }
        }

        /// <summary>
        /// 結合ピース全体のドラッグ完了処理
        /// </summary>
        public void DragCompleted()
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
                    piece.EachDragCompleted();
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
        /// 結合ピース全体のドラッグ中処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DragDelta(double horizontalChange, double verticalChange, double canvasWidth, double canvasHeight)
        {
            var tmp = CombiningPieces.ToArray();
            foreach (var piece in tmp)
            {
                piece.EachDragDelta(horizontalChange, verticalChange, canvasWidth, canvasHeight);
            }
        }

        /// <summary>
        /// 個々のドラッグ開始処理
        /// </summary>
        private void EachDragStarted()
        {
            _lastLeft = Left;
            _lastTop = Top;
        }

        /// <summary>
        /// 個々のドラッグ完了可能判定
        /// </summary>
        private bool CanCompletDrag()
        {
            // ドロップした際にピースが重ならないように
            for (int i = 0; i < _puzzle.N; i++)
            {
                for (int j = 0; j < _puzzle.N; j++)
                {
                    if (i != I || j != J)
                    {
                        var piece = _puzzle.Pieces[i, j];
                        if (Left < piece.Left + piece.Width && piece.Left + piece.Width < Left + Width + piece.Width
                            && Top < piece.Top + piece.Height && piece.Top + piece.Height < Top + Height + piece.Height)
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
        private void EachDragCompleted()
        {
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
                    Combine(csp);
                    break;
                }
                else
                {
                    MissCombin();
                    break;
                }
            }
        }

        /// <summary>
        /// 個々のドラッグ中処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EachDragDelta(double horizontalChange, double verticalChange, double canvasWidth, double canvasHeight)
        {
            var x = Left + horizontalChange;
            var y = Top + verticalChange;

            x = Math.Max(x, 0);
            y = Math.Max(y, 0);
            x = Math.Min(x, canvasWidth - Width);
            y = Math.Min(y, canvasHeight - Height);

            MoveTo(x, y);
        }

        /// <summary>
        /// 結合
        /// </summary>
        /// <returns></returns>
        private void Combine(Piece piece)
        {
            if (!CombiningPieces.Contains(piece))
            {
                CombiningPieces.AddRange(piece.CombiningPieces);
                foreach (var p in CombiningPieces)
                {
                    p.CombiningPieces = CombiningPieces;
                    var left = Left + (p.I - I) * (Width + 1);
                    var top = Top + (p.J - J) * (Height + 1);
                    p.MoveTo(left, top);
                    p.SelectCancel();
                }
            }
            Combined(this, piece);
        }

        /// <summary>
        /// 結合失敗
        /// </summary>
        private void MissCombin()
        {
            Combined(this, null);
        }

        /// <summary>
        /// コラボレーターの選択しているピースを返す
        /// </summary>
        private List<Piece> CollaboraterSelectedPieces
        {
            get
            {
                foreach (var p in _puzzle.Pieces)
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
            if (CanCombin(a.Left, a.Top, b.Left + b.Width, b.Top))
            {
                return 0;
            }
            if (CanCombin(a.Left, a.Top, b.Left, b.Top + b.Height))
            {
                return 1;
            }
            if (CanCombin(a.Left + a.Width, a.Top, b.Left, b.Top))
            {
                return 2;
            }
            if (CanCombin(a.Left, a.Top + a.Height, b.Left, b.Top))
            {
                return 3;
            }
            return -1;
        }
    }
}
