using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RemoteCollaboration.Model
{
    public class Puzzle
    {
        public Puzzle(Uri imageUri, int puzzleSize, int n, int blockMargin)
        {
            ImageUri = imageUri;
            PuzzleSize = puzzleSize;
            N = n;
            PieceMargin = blockMargin;
            Positions = new Point[N, N];
        }

        /// <summary>
        /// 画像URI
        /// </summary>
        public Uri ImageUri { get; set; }
        /// <summary>
        /// 分割数
        /// </summary>
        public int N { get; set; }
        /// <summary>
        /// パズルサイズ
        /// </summary>
        public int PuzzleSize { get; set; }
        /// <summary>
        /// ピースマージン
        /// </summary>
        public int PieceMargin { get; set; }
        /// <summary>
        /// ピースサイズ
        /// </summary>
        public int PieceSize
        {
            get
            {
                return PuzzleSize / N;
            }
        }
        /// <summary>
        /// ピースの左上座標
        /// </summary>
        public Point[,] Positions { get; set; }

        /// <summary>
        /// 初期位置をランダム生成
        /// </summary>
        /// <param name="canvasWidth"></param>
        /// <param name="canvasHeight"></param>
        public void RandomizePositions(int canvasWidth, int canvasHeight)
        {
            int length = PieceSize + PieceMargin;
            int maxColumn = canvasWidth / length;
            int maxRow = canvasHeight / length;
            var randList = GetUniqRandomNumber(0, maxColumn * maxRow - 1, N * N).ToList();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    var left = randList[i * N + j] % maxColumn * length;
                    var top = randList[i * N + j] / maxColumn * length;
                    Positions[i, j] = new Point(left, top);
                }
            }
        }

        static IEnumerable<int> GetUniqRandomNumber(int rangeBegin, int rangeEnd, int count)
        {
            int[] work = new int[rangeEnd - rangeBegin + 1];
            
            for (int n = rangeBegin, i = 0; n <= rangeEnd; n++, i++)
                work[i] = n;
            
            var rnd = new Random();
            for (int resultPos = 0; resultPos < count; resultPos++)
            {
                int nextResultPos = rnd.Next(resultPos, work.Length);
                
                Swap(ref work[resultPos], ref work[nextResultPos]);
            }

            return work.Take(count);
        }

        static void Swap(ref int m, ref int n)
        {
            int work = m;
            m = n;
            n = work;
        }
    }
}
