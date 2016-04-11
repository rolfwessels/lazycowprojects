using System;

namespace Cgol.Core
{
    public class Rule : IRuleValueToLookAt, IRuleActionToTake, IRule, IRuleNeighbourCounts
    {
        private bool _initialialState;
        private Func<int, bool> _satisfy;
        private bool _valueSet;

        #region IRule Members

        public bool Process(Cell cell, int neighbour)
        {
            if (cell.IsOn == _initialialState && _satisfy(neighbour))
            {
                cell.IsOn = _valueSet;
                return true;
            }
            return false;
        }

        #endregion

        #region IRuleActionToTake Members

        public Rule Dies()
        {
            _valueSet = false;
            return this;
        }

        public Rule Lives()
        {
            _valueSet = true;
            return this;
        }

        #endregion

        #region IRuleNeighbourCounts Members

        public IRuleValueToLookAt CellWithFewerThan(int i)
        {
            _satisfy = count => count < i;
            return this;
        }

        public IRuleValueToLookAt CellWithTwoOrThree()
        {
            _satisfy = count => count == 2 || count == 3;
            return this;
        }

        public IRuleValueToLookAt CellWithMoreThan(int i)
        {
            _satisfy = count => count > i;
            return this;
        }

        public IRuleValueToLookAt CellWithExactly(int i)
        {
            _satisfy = count => count == i;
            return this;
        }

        #endregion

        #region IRuleValueToLookAt Members

        public IRuleActionToTake Neighbours()
        {
            return this;
        }

        #endregion

        public IRuleNeighbourCounts Live()
        {
            _initialialState = true;
            return this;
        }

        public IRuleNeighbourCounts Dead()
        {
            _initialialState = false;
            return this;
        }
    }
}