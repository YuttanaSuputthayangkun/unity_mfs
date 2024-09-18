using Characters.Interfaces;
using Data;
using UnityEngine;

namespace Characters
{
    public class Obstacle : ICharacter
    {
        private readonly CharacterComponent _characterComponent;
        private readonly IReadOnlyCharacterData<ObstacleType> _readOnlyCharacterData;
        private readonly MoveCharacterHandler _moveCharacterHandler;
        private readonly RemoveCharacterHandler _removeCharacterHandler;

        public Obstacle(
            CharacterComponent characterComponent,
            IReadOnlyCharacterData<ObstacleType> readOnlyCharacterData,
            MoveCharacterHandler moveCharacterHandler,
            RemoveCharacterHandler removeCharacterHandler
        )
        {
            _characterComponent = characterComponent;
            _readOnlyCharacterData = readOnlyCharacterData;
            _moveCharacterHandler = moveCharacterHandler;
            _removeCharacterHandler = removeCharacterHandler;
        }

        public override string ToString()
        {
            return $"{_characterComponent.gameObject.name} Data({_readOnlyCharacterData})";
        }

        public CharacterType GetCharacterType() => CharacterType.Obstacle;
        
        public bool IsPlayerCharacter() => false;

        public MoveResultType TryMove(BoardCoordinate coordinate) => _moveCharacterHandler.TryMove(coordinate, this);

        public BoardCoordinate? GetBoardCoordinate()
        {
            throw new System.NotImplementedException();
        }

        public void SetWorldPosition(Vector3 worldPosition)
        {
            _characterComponent.transform.position = worldPosition;
        }
        
        public void Remove() => _removeCharacterHandler.RemoveCharacter(this);
    }
}