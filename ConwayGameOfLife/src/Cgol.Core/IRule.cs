namespace Cgol.Core
{
    public interface IRule
    {
        bool Process(Cell cell, int neighbour);
    }
}