using Characters.Interfaces;
using Data;
using UnityEngine;

namespace Characters
{
    public class Obstacle :
        IContainBoardObjectType
        , ISetWorldPosition
    {
        private readonly CharacterComponent _characterComponent;
        private readonly IReadOnlyCharacterData<ObstacleType> _readOnlyCharacterData;

        public BoardObjectType BoardObjectType => BoardObjectType.Obstacle;

        public Obstacle(
            CharacterComponent characterComponent,
            IReadOnlyCharacterData<ObstacleType> readOnlyCharacterData
        )
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