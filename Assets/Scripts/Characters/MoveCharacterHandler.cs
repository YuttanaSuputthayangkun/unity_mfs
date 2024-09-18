using Board;
using Characters.Interfaces;
using Data;

namespace Characters
{
    public class MoveCharacterHandler
    {
        private readonly BoardManager _boardManager;

        public MoveCharacterHandler(
            BoardManager boardManager
        )
        {
            _boardManager = boardManager;
        }

        public MoveResultType TryMove(BoardCoordinate nextBoardCoordinate, ICharacter character)
        {
            var currentCoordinate = character.GetBoardCoordinate();
            var getCellResult = _boardManager.GetCell(nextBoardCoordinate);
            if (getCellResult.CellData is null)
            {
                return MoveResultType.OutOfBound;
            }

            if (getCellResult.CellData.Character is not null)
            {
                return MoveResultType.OccupiedByOtherObject;
            }

            var nextWorldPosition = getCellResult.CellData.WorldPosition;
            _boardManager.MoveCharacter(nextBoardCoordinate, character);
            character.SetWorldPosition(nextWorldPosition);

            return MoveResultType.Success;
        }
    }
}