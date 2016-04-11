using System;
using System.Collections.Generic;
using System.Linq;

namespace Cgol.Core
{
    public class ProcessRules
    {
        private readonly IRule[] _rules;


        public ProcessRules(params IRule[] rules)
        {
            _rules = rules;
        }

        /// <summary>
        /// Any live cell with fewer than two live neighbours dies, as if caused by under-population.
        /// Any live cell with two or three live neighbours lives on to the next generation.
        /// Any live cell with more than three live neighbours dies, as if by overcrowding.
        /// Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
        /// </summary>
        public static ProcessRules DefaultRules()
        {
            return new ProcessRules(
                new Rule().Live().CellWithFewerThan(2).Neighbours().Dies(),
                new Rule().Live().CellWithTwoOrThree().Neighbours().Lives(),
                new Rule().Live().CellWithMoreThan(3).Neighbours().Dies(),
                new Rule().Dead().CellWithExactly(3).Neighbours().Lives()
                );
        }


        public List<Cell> ProcessAllRules(GameMatrix matrix)
        {
            var changes = new List<Cell>(matrix.Width*matrix.Height);
            Cell[] allThatAreOn = matrix.Cells.Where(x => x.IsOn).ToArray();
            foreach (Cell cell in allThatAreOn.Union(allThatAreOn.SelectMany(x => x.Neighbour(matrix))).ToArray())
            {
                int neighbours = cell.Neighbour(matrix).Where(x => x.IsOn).Count();
                foreach (IRule rule in _rules)
                {
                    Cell nCell = cell.Clone();
                    if (rule.Process(nCell, neighbours))
                    {
                        changes.Add(nCell);
                    }
                }
            }
            Console.Out.WriteLine(string.Format("{0} changes", changes.Count));
            matrix.ApplyChanges(changes);
            return changes;
        }
    }
}