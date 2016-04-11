using System;
using System.Collections.Generic;
using System.Linq;

namespace Cgol.Core
{
    public static class GameMatrixHelper
    {
        public static IEnumerable<Cell> Dump(this IEnumerable<Cell> matrix, string isactive)
        {
            Console.Out.WriteLine(isactive);
            Console.Out.WriteLine("----------------------");
            return Dump(matrix);
        }

        public static IEnumerable<Cell> Dump(this IEnumerable<Cell> matrix)
        {
            foreach (Cell cell in matrix)
            {
                Console.Out.WriteLine(cell.ToString());
            }
            return matrix;
        }

        public static IEnumerable<Cell> GetCells(this IEnumerable<Cell> matrix, IEnumerable<Point> points)
        {
            return matrix.Where(cell => points.Contains(cell.Point));
        }

        public static IEnumerable<Cell> GetOtherCells(this IEnumerable<Cell> matrix, IEnumerable<Point> points)
        {
            return matrix.Where(cell => !points.Contains(cell.Point));
        }

        public static bool IsAllAlive(this IEnumerable<Cell> cells)
        {
            return !cells.Where(x => x.IsOn == false).Any();
        }

        public static bool IsAllInDead(this IEnumerable<Cell> cells)
        {
            return !cells.Where(x => x.IsOn).Any();
        }

        public static IEnumerable<Cell> CellsNotInMatrix(this IEnumerable<Cell> cells, IEnumerable<Cell> matrix)
        {
            return matrix.Where(cell => !cells.Where(x => x.Point == cell.Point).Any());
        }
    }
}