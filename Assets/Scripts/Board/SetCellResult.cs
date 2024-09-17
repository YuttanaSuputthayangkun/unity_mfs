#nullable enable

namespace Board
{
    public partial class BoardManager
    {
        public enum SetCellResultType
        {
            Set,
            OutOfBound,
        }

        public struct SetCellResult
        {
            public SetCellResultType ResultType;
            public bool IsSuccess => ResultType == SetCellResultType.Set;

            public override string ToString()
            {
                return ResultType.ToString();
            }
        }
    }
}
