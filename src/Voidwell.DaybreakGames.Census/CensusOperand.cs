using System;

namespace Voidwell.DaybreakGames.Census
{
    public sealed class CensusOperand
    {
        private object _comparator { get; set; }
        private OperatorType _operator { get; set; }

        public new void Equals(object value)
        {
            _comparator = value;
            _operator = OperatorType.Equals;
        }

        public void NotEquals(object value)
        {
            _comparator = value;
            _operator = OperatorType.NotEquals;
        }

        public void IsLessThan(object value)
        {
            _comparator = value;
            _operator = OperatorType.IsLessThan;
        }

        public void IsLessThanOrEquals(object value)
        {
            _comparator = value;
            _operator = OperatorType.IsLessThanOrEquals;
        }

        public void IsGreaterThan(object value)
        {
            _comparator = value;
            _operator = OperatorType.IsGreaterThan;
        }

        public void IsGreaterThanOrEquals(object value)
        {
            _comparator = value;
            _operator = OperatorType.IsGreaterThanOrEquals;
        }

        public void StartsWith(object value)
        {
            _comparator = value;
            _operator = OperatorType.StartsWith;
        }

        public void Contains(object value)
        {
            _comparator = value;
            _operator = OperatorType.Contains;
        }

        public override string ToString()
        {
            var mod = "";

            switch(_operator)
            {
                case OperatorType.NotEquals:
                    mod = "!";
                    break;
                case OperatorType.IsLessThan:
                    mod = "<";
                    break;
                case OperatorType.IsLessThanOrEquals:
                    mod = "[";
                    break;
                case OperatorType.IsGreaterThan:
                    mod = ">";
                    break;
                case OperatorType.IsGreaterThanOrEquals:
                    mod = "]";
                    break;
                case OperatorType.StartsWith:
                    mod = "^";
                    break;
                case OperatorType.Contains:
                    mod = "*";
                    break;
            }

            return $"={mod}{GetComparatorString()}";
        }

        private string GetComparatorString()
        {
            if (_comparator is DateTime value)
            {
                return value.ToString("yyyy-MM-dd HH\\:mm\\:ss");
            }

            return _comparator.ToString();
        }

        internal enum OperatorType
        {
            Equals,
            NotEquals,
            IsLessThan,
            IsLessThanOrEquals,
            IsGreaterThan,
            IsGreaterThanOrEquals,
            StartsWith,
            Contains
        }
    }
}
