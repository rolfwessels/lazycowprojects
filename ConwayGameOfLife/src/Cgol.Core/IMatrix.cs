using System.Collections.Generic;

namespace Cgol.Core
{
    public interface IMatrix
    {
        IEnumerable<Cell> Neighbour(GameMatrix matrix);
    }
}