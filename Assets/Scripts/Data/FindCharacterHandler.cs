using System;
using Board;
using Characters;
using Characters.Interfaces;

namespace Data
{
    public class FindCharacterHandler 
    {
        public struct Result
        {
            public ICharacter Character;
            public bool IsPlayerCharacter;
        } 
        
        private readonly BoardManager _boardManager;
        private readonly HeroRow _heroRow;

        public FindCharacterHandler(
            BoardManager boardManager,
            HeroRow heroRow
        )
        {
            _boardManager = boardManager;
            _heroRow = heroRow;
        }

        public Result Find(BoardCoordinate boardCoordinate)
        {
            var getCellResult = _boardManager.GetCell(boardCoordinate);
            if (getCellResult.CellData is null)
            {
                // cell not found
                return new Result();
            }

            if (getCellResult.CellData.Character is null)
            {
                // cell is empty
                return new Result();
            }

            // if (_heroRow.)
            // {
            //     
            // }
            throw new NotImplementedException();
        }
    }
}
