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
        , ISetCharacterStats
    {
        private readonly CharacterComponent _characterComponent;
        private readonly MoveCharacterHandler _moveCharacterHandler;
        private readonly RemoveCharacterHandler _removeCharacterHandler;
        private readonly LocateCharacterHandler _locateCharacterHandler;
        private readonly EnemyData _enemyData;

        public Enemy(
            CharacterComponent characterComponent,
            IReadOnlyCharacterData<EnemyType> readOnlyCharacterData,
            MoveCharacterHandler moveCharacterHandler,
            RemoveCharacterHandler removeCharacterHandler,
            LocateCharacterHandler locateCharacterHandler
        )
        {
            _characterComponent = characterComponent;
            _enemyData = new EnemyData(readOnlyCharacterData);
            _moveCharacterHandler = moveCharacterHandler;
            _removeCharacterHandler = removeCharacterHandler;
            _locateCharacterHandler = locateCharacterHandler;

            SetCharacterStats(this);
        }

        public override string ToString()
        {
            return $"{_characterComponent.gameObject.name} Data({_enemyData})";
        }

        public void SetCharacterStats(ICharacterStats characterStats)
        {
            _enemyData.SetStats(characterStats);
            _characterComponent.SetStats(characterStats);
        }

        public void SetNumber(int? number) => _characterComponent.SetNumber(number);

        public CharacterType GetCharacterType() => CharacterType.Enemy;

        public EnemyType EnemyType => _enemyData.Type;

        public bool IsPlayerCharacter() => false;

        public int Health => _enemyData.Stats.health;
        public int Attack => _enemyData.Stats.attack;
        public int Defense => _enemyData.Stats.defense;

        public MoveResultType TryMove(BoardCoordinate coordinate) => _moveCharacterHandler.TryMove(coordinate, this);

        public BoardCoordinate? GetBoardCoordinate() => _locateCharacterHandler.LocateCharacter(this);

        public void SetWorldPosition(Vector3 worldPosition)
        {
            _characterComponent.transform.position = worldPosition;
        }

        public void Remove() => _removeCharacterHandler.RemoveCharacter(this);
    }
}