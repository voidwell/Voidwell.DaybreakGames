namespace Voidwell.DaybreakGames.Census
{
    public sealed class CensusOperand
    {
        private string _comparator { get; set; }
        private OperatorType _operator { get; set; }

        public void Equals(string value)
        {
            _comparator = value;
            _operator = OperatorType.Equals;
        }

        public void NotEquals(string value)
        {
            _comparator = value;
            _operator = OperatorType.NotEquals;
        }

        public void IsLessThan(string value)
        {
            _comparator = value;
            _operator = OperatorType.IsLessThan;
        }

        public void IsLessThanOrEquals(string value)
        {
            _comparator = value;
            _operator = OperatorType.IsLessThanOrEquals;
        }

        public void IsGreaterThan(string value)
        {
            _comparator = value;
            _operator = OperatorType.IsGreaterThan;
        }

        public void IsGreaterThanOrEquals(string value)
        {
            _comparator = value;
            _operator = OperatorType.IsGreaterThanOrEquals;
        }

        public void StartsWith(string value)
        {
            _comparator = value;
            _operator = OperatorType.StartsWith;
        }

        public void Contains(string value)
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

            return $"={mod}{_comparator}";
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
