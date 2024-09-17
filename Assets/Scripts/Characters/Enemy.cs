using Characters.Interfaces;
using Data;
using UnityEngine;

namespace Characters
{
    public class Enemy :
        IContainBoardObjectType
        , ISetWorldPosition
    {
        private readonly CharacterComponent _characterComponent;
        private readonly IReadOnlyCharacterData<EnemyType> _readOnlyCharacterData;

        public BoardObjectType BoardObjectType => BoardObjectType.Enemy;

        public Enemy(CharacterComponent characterComponent, IReadOnlyCharacterData<EnemyType> readOnlyCharacterData)
        {
            _characterComponent = characterComponent;
            _readOnlyCharacterData = readOnlyCharacterData;
        }

        public override string ToString()
        {
            return $"Component({_characterComponent.GetHashCode()}) Data({_readOnlyCharacterData})";
        }

        public void SetWorldPosition(Vector3 worldPosition)
        {
            _characterComponent.transform.position = worldPosition;
        }
    }
}