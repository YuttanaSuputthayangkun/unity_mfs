#nullable enable

namespace Board
{
    public partial class BoardManager
    {
        public enum GetCellResultType
        {
            Found,
            OutOfBound,
            NoCharacterOnBoard,
        }

        public struct GetCellResult
        {
            public GetCellResultType ResultType;
            public bool IsFound => ResultType == GetCellResultType.Found;
            public IReadOnlyCellData? CellData;

            public override string ToString()
            {
                return $"{ResultType} {CellData}";
            }
        }
    }
}
