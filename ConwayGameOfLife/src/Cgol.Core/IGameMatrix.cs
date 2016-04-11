using System.Collections.Generic;

namespace Cgol.Core
{
    public interface IGameMatrix
    {
        IEnumerable<Cell> Cells { get; }
    }
}