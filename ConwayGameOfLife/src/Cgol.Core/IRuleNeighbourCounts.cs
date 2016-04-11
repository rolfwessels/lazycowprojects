namespace Cgol.Core
{
    public interface IRuleNeighbourCounts
    {
        IRuleValueToLookAt CellWithTwoOrThree();
        IRuleValueToLookAt CellWithMoreThan(int i);
        IRuleValueToLookAt CellWithExactly(int i);
        IRuleValueToLookAt CellWithFewerThan(int i);
    }
}