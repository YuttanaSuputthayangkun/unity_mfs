using Characters.Interfaces;
using Data;
using UnityEngine;

namespace Characters
{
    public class Enemy : 
        ICharacter
        , ISetNumber
    {
        private readonly CharacterComponent _characterComponent;
        private readonly IReadOnlyCharacterData<EnemyType> _readOnlyCharacterData;
        private readonly CharacterMoveHandler _characterMoveHandler;
        private readonly RemoveCharacterHandler _removeCharacterHandler;

        public Enemy(
            CharacterComponent characterComponent,
            IReadOnlyCharacterData<EnemyType> readOnlyCharacterData,
            CharacterMoveHandler characterMoveHandler,
            RemoveCharacterHandler removeCharacterHandler
        )
        {
            _characterComponent = characterComponent;
            _readOnlyCharacterData = readOnlyCharacterData;
            _characterMoveHandler = characterMoveHandler;
            _removeCharacterHandler = removeCharacterHandler;
        }

        public override string ToString()
        {
            return $"{_characterComponent.gameObject.name} Data({_readOnlyCharacterData})";
        }

        public void SetNumber(int? number) => _characterComponent.SetNumber(number);

        public CharacterType GetCharacterType() => CharacterType.Enemy;

        public MoveResultType TryMove(BoardCoordinate coordinate) => _characterMoveHandler.TryMove(coordinate, this);

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