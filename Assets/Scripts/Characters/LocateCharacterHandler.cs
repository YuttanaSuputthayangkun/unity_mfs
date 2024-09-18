using Board;
using Characters.Interfaces;
using Data;

namespace Characters
{
    public class LocateCharacterHandler 
    {
        private readonly BoardManager _boardManager;

        public LocateCharacterHandler(BoardManager boardManager)
        {
            _boardManager = boardManager;
        }
        
        public BoardCoordinate? LocateCharacter(ICharacter character)
        {
            var getCellResult = _boardManager.GetCell(character);
            return getCellResult.CellData?.Coordinate;
        }
    }
}
