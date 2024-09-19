using Characters.Interfaces;
using Data;
using UnityEngine;

namespace Characters
{
    public class Enemy :
        ICharacter
        , ISetNumber
        , IContainEnemyType
        , ICharacterStats
    {
        private readonly CharacterComponent _characterComponent;
        private readonly MoveCharacterHandler _moveCharacterHandler;
        private readonly RemoveCharacterHandler _removeCharacterHandler;
        private readonly EnemyData _enemyData;

        public Enemy(
            CharacterComponent characterComponent,
            IReadOnlyCharacterData<EnemyType> readOnlyCharacterData,
            MoveCharacterHandler moveCharacterHandler,
            RemoveCharacterHandler removeCharacterHandler
        )
        {
            _characterComponent = characterComponent;
            _enemyData = new EnemyData(readOnlyCharacterData);
            _moveCharacterHandler = moveCharacterHandler;
            _removeCharacterHandler = removeCharacterHandler;
        }

        public override string ToString()
        {
            return $"{_characterComponent.gameObject.name} Data({_enemyData})";
        }

        public void SetNumber(int? number) => _characterComponent.SetNumber(number);

        public CharacterType GetCharacterType() => CharacterType.Enemy;

        public EnemyType EnemyType => _enemyData.Type;

        public bool IsPlayerCharacter() => false;

        public int Health => _enemyData.Stats.health;
        public int Attack => _enemyData.Stats.attack;
        public int Defense => _enemyData.Stats.defense;

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