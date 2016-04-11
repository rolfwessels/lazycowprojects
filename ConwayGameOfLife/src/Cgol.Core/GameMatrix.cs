using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cgol.Core
{
    public class GameMatrix : IGameMatrix, IEnumerable<Cell>
    {
        private readonly List<List<Cell>> _list;
        private readonly Random _random = new Random();

        public GameMatrix(int width, int height)
        {
            Width = width;
            Height = height;
            _list = new List<List<Cell>>();
            PupulateMatrix();
        }

        public Cell this[int x, int y]
        {
            get { return _list[x][y]; }
            set { _list[x][y] = value; }
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        #region IEnumerable<Cell> Members

        public IEnumerator<Cell> GetEnumerator()
        {
            return _list.SelectMany(x => x).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IGameMatrix Members

        public IEnumerable<Cell> Cells
        {
            get { return _list.SelectMany(x => x); }
        }

        #endregion

        #region Private Methods

        #endregion

        private void PupulateMatrix()
        {
            int count = _list.Count;
            for (int x = count; x < Width; x++)
            {
                _list.Add(new List<Cell>());
                for (int y = _list[x].Count; y < Height; y++)
                {
                    _list[x].Add(new Cell(new Point(x, y)));
                }
            }
        }

        public void Activate(params KeyValuePair<int, int>[] points)
        {
            Activate(points.Select(x => new Point(x.Key, x.Value)).ToArray());
        }

        public GameMatrix Activate(params Point[] points)
        {
            foreach (Cell cell in Cells)
            {
                if (points.Contains(cell.Point))
                {
                    cell.IsOn = true;
                }
            }
            return this;
        }

        public IEnumerable<Point> RandomPoints()
        {
            while (true)
            {
                yield return new Point(_random.Next(0, Width - 1), _random.Next(0, Height - 1));
            }
        }

        public void ApplyChanges(List<Cell> changes)
        {
            foreach (Cell change in changes)
            {
                _list[change.Point.X][change.Point.Y] = change;
            }
        }
    }
}