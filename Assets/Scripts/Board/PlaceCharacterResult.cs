using System;

#nullable enable

namespace Board
{
    public partial class BoardManager
    {
        public enum PlaceCharacterResultType
        {
            Placed,
            OutOfBound,
            CellOccupied,
        }

        public struct PlaceCharacterResult
        {
            public PlaceCharacterResultType ResultType;

            public bool IsSuccess => ResultType == PlaceCharacterResultType.Placed;

            public override string ToString()
            {
                return ResultType.ToString();
            }
        }
    }
}
