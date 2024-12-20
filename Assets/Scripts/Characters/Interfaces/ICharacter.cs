using Data;
using Data.Interfaces;
using IContainCharacterType = Data.Interfaces.IContainCharacterType;

#nullable enable

namespace Characters.Interfaces
{
    public interface IReadOnlyCharacter : IContainBoardCoordinate
        , IContainCharacterType
    {
    }

    public interface ICharacter : IReadOnlyCharacter
        , ISetWorldPosition
    {
        MoveResultType TryMove(BoardCoordinate coordinate);
        void Remove();
        bool IsPlayerCharacter();
    }
}