#nullable enable

namespace Board
{
    public partial class BoardManager
    {
        public enum MoveCharacterResultType
        {
            Moved,
            OutOfBound,
            CellOccupied,
            CharacterIsNotOnBoard,
            SameCoordinate,
        }

        public struct MoveCharacterResult
        {
            public MoveCharacterResultType ResultType;
            public bool IsSuccess => ResultType == MoveCharacterResultType.Moved;

            public override string ToString()
            {
                return ResultType.ToString();
            }
        }
    }
}
