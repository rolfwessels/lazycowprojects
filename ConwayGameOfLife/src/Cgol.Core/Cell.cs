using System.Collections.Generic;

namespace Cgol.Core
{
    public class Cell : IMatrix
    {
        public Cell(Point point)
        {
            Point = point;
        }

        public bool IsOn { get; set; }

        public Point Point { get; private set; }

        #region IMatrix Members

        public IEnumerable<Cell> Neighbour(GameMatrix matrix)
        {
            for (int x = Point.X - 1; x <= Point.X + 1; x++)
            {
                for (int y = Point.Y - 1; y <= Point.Y + 1; y++)
                {
                    if (x >= 0 && y >= 0 && x < matrix.Width && y < matrix.Height && !(x == Point.X && y == Point.Y))
                    {
                        yield return matrix[x, y];
                    }
                }
            }
        }

        #endregion

        public Cell Clone()
        {
            return new Cell(Point) {IsOn = IsOn};
        }

        public bool Equals(Cell other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.IsOn.Equals(IsOn) && Equals(other.Point, Point);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Cell)) return false;
            return Equals((Cell) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (IsOn.GetHashCode()*397) ^ (Point != null ? Point.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}-{2}", Point.X, Point.Y, IsOn);
        }
    }
}