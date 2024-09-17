using Data;
using UnityEngine;

#nullable enable

namespace Characters
{
    public class Hero : IContainBoardObjectType
    {
        private readonly CharacterComponent _characterComponent;
        private readonly HeroData _heroData;
        public BoardObjectType BoardObjectType => Data.BoardObjectType.Hero;

        public Hero(CharacterComponent characterComponent, IReadOnlyCharacterData<HeroType> readOnlyCharacterData)
        {
            _characterComponent = characterComponent;
            _heroData = new HeroData(readOnlyCharacterData);
        }

        public override string ToString()
        {
            return $"Component({_characterComponent.GetHashCode()}) Data({_heroData})";
        }
        
        public void SetWorldPosition(Vector3 worldPosition)
        {
            _characterComponent.transform.position = worldPosition;
        }
    }
}
