using System.Linq;
using Cgol.Core;
using NUnit.Framework;

namespace Cgol.Tests
{
    [TestFixture]
    public class ProcessRulesTests
    {
        /// <summary>
        /// Any live cell with fewer than two live neighbours dies, as if caused by under-population.
        /// </summary>
        [Test]
        public void ProcessAllRules_AnyLiveCellWithFewerThanTwoLiveNeighboursDies_IsDead()
        {
            var processRules = new ProcessRules(new Rule().Live().CellWithFewerThan(2).Neighbours().Dies());
            var gameMatrix = new GameMatrix(6, 6).Activate(new [] { new Point(2,3) });
            Assert.That(gameMatrix[2,3].IsOn,"Initialized Correctly");
            processRules.ProcessAllRules(gameMatrix);
            Assert.That(gameMatrix[2, 3].IsOn,Is.False,"It dies was removed");
        }


        /// <summary>
        /// Any live cell with fewer than two live neighbours dies, as if caused by under-population.
        /// </summary>
        [Test]
        public void ProcessAllRules_DefaultRules_ShouldBeValid()
        {
            var processRules = ProcessRules.DefaultRules();
            var gameMatrix = new GameMatrix(6, 6).Activate(new[] { new Point(2, 3) });
            Assert.That(gameMatrix[2, 3].IsOn, "Initialized Correctly");
            processRules.ProcessAllRules(gameMatrix);
            Assert.That(gameMatrix[2, 3].IsOn, Is.False, "It dies was removed");
        }

        /// <summary>
        /// Any live cell with two or three live neighbours lives on to the next generation.
        /// </summary>
        [Test]
        public void ProcessAllRules_AnyLiveCellWithTwoLiveNeighbours_ShouldLiveOn()
        {
            var processRules = new ProcessRules(
                new Rule().Live().CellWithTwoOrThree().Neighbours().Lives());
            var activate = new[] { new Point(2, 2), new Point(2, 3), new Point(3, 2), new Point(3, 3) };
            var gameMatrix = new GameMatrix(6, 6).Activate(activate);

            var processAllRules = processRules.ProcessAllRules(gameMatrix);
            var shouldBeActive = new[] { new Point(2, 2), new Point(2, 3), new Point(3, 2), new Point(3, 3) };

            Assert.That(processAllRules.Count,Is.EqualTo(4), "Changes");
            Assert.That(gameMatrix.GetCells(shouldBeActive).IsAllAlive(), "All Points are alive");
        }

        /// <summary>
        /// Any live cell with two or three live neighbours lives on to the next generation.
        /// </summary>
        [Test]
        public void ProcessAllRules_AnyLiveCellWithThreeLiveNeighbours_ShouldLiveOn()
        {
            var processRules = new ProcessRules(
                new Rule().Live().CellWithTwoOrThree().Neighbours().Lives());
            var activate = new[] { new Point(2, 2), new Point(2, 3), new Point(3, 2), new Point(3, 3) };
            var gameMatrix = new GameMatrix(6, 6).Activate(activate);

            var processAllRules = processRules.ProcessAllRules(gameMatrix);
            var shouldBeActive = new[] { new Point(2, 2), new Point(2, 3), new Point(3, 2), new Point(3, 3) };

            Assert.That(processAllRules.Count, Is.EqualTo(4), "Changes");
            Assert.That(gameMatrix.GetCells(shouldBeActive).IsAllAlive(), "All Points are alive");
        }


        /// <summary>
        /// Any live cell with more than three live neighbours dies, as if by overcrowding.
        /// </summary>
        [Test]
        public void ProcessAllRules_AnyLiveCellWithMoreThanThreeLiveNeighbours_Dies()
        {
            var processRules = new ProcessRules(new Rule().Live().CellWithMoreThan(3).Neighbours().Dies());
            var activate = new[] { new Point(2, 2), new Point(2, 3), new Point(2, 4), new Point(3, 2), new Point(3, 3), new Point(3, 4) };
            var gameMatrix = new GameMatrix(6, 6).Activate(activate);

            var processAllRules = processRules.ProcessAllRules(gameMatrix);
            var shouldBeActive = new[] { new Point(2, 2), new Point(2, 4), new Point(3, 2), new Point(3, 4) };

            Assert.That(processAllRules.Count, Is.EqualTo(2), "Changes");
            Assert.That(gameMatrix.GetCells(shouldBeActive).IsAllAlive(), "All Points are alive");
            
        }

        /// <summary>
        /// Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
        /// </summary>
        [Test]
        public void ProcessAllRules_AnyDeadCellWithExactlyThreeLiveNeighbours_BecomesLiveCell()
        {
            var processRules = new ProcessRules(new Rule().Dead().CellWithExactly(3).Neighbours().Lives());
            var activate = new[] { new Point(2, 2), new Point(3, 3), new Point(2, 4)};
            var gameMatrix = new GameMatrix(6, 6).Activate(activate);

            var processAllRules = processRules.ProcessAllRules(gameMatrix);
            var shouldBeActive = new[] { new Point(2, 2), new Point(2, 3), new Point(2, 4), new Point(3, 3) };

            Assert.That(processAllRules.Count, Is.EqualTo(1), "Changes");
            Assert.That(gameMatrix.GetCells(shouldBeActive).IsAllAlive(), "All Points are alive");
        }

        /// <summary>
        /// Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
        /// </summary>
        [Test]
        public void ProcessAllRules_AnyDeadCellWithExactlyThreeLiveNeighbours_NoChanges()
        {
            var processRules = new ProcessRules(new Rule().Dead().CellWithExactly(3).Neighbours().Lives());
            var activate = new[] { new Point(2, 2), new Point(2, 3), new Point(2, 4) };
            var gameMatrix = new GameMatrix(6, 6).Activate(activate);

            var processAllRules = processRules.ProcessAllRules(gameMatrix);
            var shouldBeActive = new[] { new Point(2, 2), new Point(2, 3), new Point(2, 4), new Point(1, 3), new Point(3, 3) };

            Assert.That(processAllRules.Count, Is.EqualTo(2), "Changes");
            Assert.That(gameMatrix.GetCells(shouldBeActive).IsAllAlive(), "All Points are alive");
        }
    }

}