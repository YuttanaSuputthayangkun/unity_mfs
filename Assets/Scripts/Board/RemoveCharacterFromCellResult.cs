using System;
using Characters.Interfaces;

#nullable enable

namespace Board
{
    public partial class BoardManager
    {
        public enum RemoveCharacterFromCellResultType
        {
            Removed,
            OutOfBound,
            CharacterNotFound,
        }

        public struct RemoveCharacterFromCellResult
        {
            public RemoveCharacterFromCellResultType ResultType;
            public ICharacter? RemovedCharacter;

            public bool IsSuccess => ResultType switch
            {
                RemoveCharacterFromCellResultType.Removed => true,
                RemoveCharacterFromCellResultType.OutOfBound => false,
                RemoveCharacterFromCellResultType.CharacterNotFound when RemovedCharacter is { } => true,
                _ => throw new ArgumentOutOfRangeException()
            };

            public override string ToString()
            {
                return ResultType.ToString();
            }
        }
    }
}