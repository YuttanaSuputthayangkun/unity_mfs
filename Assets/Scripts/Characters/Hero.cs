using System;
using Characters.Interfaces;
using Data;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

namespace Characters
{
    public class Hero :
        ICharacter
        , ISetNumber
        , IContainHeroType
        , ICharacterStats
        , ISetCharacterStats
    {
        private readonly CharacterComponent _characterComponent;
        private readonly MoveCharacterHandler _moveCharacterHandler;
        private readonly LocateCharacterHandler _locateCharacterHandler;
        private readonly RemoveCharacterHandler _removeCharacterHandler;
        private readonly Func<ICharacter, bool> _isPlayerCharacter;
        private readonly HeroData _heroData;

        private int? number = null;

        public Hero(
            CharacterComponent characterComponent,
            IReadOnlyCharacterData<HeroType> readOnlyCharacterData,
            MoveCharacterHandler moveCharacterHandler,
            LocateCharacterHandler locateCharacterHandler,
            RemoveCharacterHandler removeCharacterHandler,
            Func<ICharacter, bool> isPlayerCharacter
        )
        {
            _characterComponent = characterComponent;
            _moveCharacterHandler = moveCharacterHandler;
            _locateCharacterHandler = locateCharacterHandler;
            _removeCharacterHandler = removeCharacterHandler;
            _isPlayerCharacter = isPlayerCharacter;
            _heroData = new HeroData(readOnlyCharacterData);
        }

        public override string ToString()
        {
            return $"{_characterComponent.gameObject.name} Data({_heroData}) Number({number})";
        }

        public void SetCharacterStats(ICharacterStats characterStats) => _heroData.SetStats(characterStats);

        public MoveResultType TryMove(BoardCoordinate coordinate) => _moveCharacterHandler.TryMove(coordinate, this);

        public HeroType HeroType => _heroData.Type;
        
        public int Health => _heroData.Stats.health;
        public int Attack => _heroData.Stats.attack;
        public int Defense => _heroData.Stats.defense;
        
        public void Remove() => _removeCharacterHandler.RemoveCharacter(this);

        public bool IsPlayerCharacter() => _isPlayerCharacter.Invoke(this);

        public void SetNumber(int? number)
        {
            _characterComponent.SetNumber(number);
            this.number = number;
        }

        public CharacterType GetCharacterType() => CharacterType.Hero;

        public BoardCoordinate? GetBoardCoordinate()
        {
            return _locateCharacterHandler.LocateCharacter(this);
        }

        public void SetWorldPosition(Vector3 worldPosition)
        {
            _characterComponent.transform.position = worldPosition;
        }
    }
}