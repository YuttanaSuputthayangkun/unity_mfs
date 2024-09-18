using Characters.Interfaces;
using Data;
using UnityEngine;

#nullable enable

namespace Characters
{
    public class Hero :
        ICharacter
        , ISetNumber
    {
        private readonly CharacterComponent _characterComponent;
        private readonly CharacterMoveHandler _characterMoveHandler;
        private readonly LocateCharacterHandler _locateCharacterHandler;
        private readonly RemoveCharacterHandler _removeCharacterHandler;
        private readonly HeroData _heroData;

        public Hero(
            CharacterComponent characterComponent,
            IReadOnlyCharacterData<HeroType> readOnlyCharacterData,
            CharacterMoveHandler characterMoveHandler,
            LocateCharacterHandler locateCharacterHandler,
            RemoveCharacterHandler removeCharacterHandler
        )
        {
            _characterComponent = characterComponent;
            _characterMoveHandler = characterMoveHandler;
            _locateCharacterHandler = locateCharacterHandler;
            _removeCharacterHandler = removeCharacterHandler;
            _heroData = new HeroData(readOnlyCharacterData);
        }

        public override string ToString()
        {
            return $"{_characterComponent.gameObject.name} Data({_heroData})";
        }

        public MoveResultType TryMove(BoardCoordinate coordinate) => _characterMoveHandler.TryMove(coordinate, this);
        public void Remove() => _removeCharacterHandler.RemoveCharacter(this);
        public void SetNumber(int? number) => _characterComponent.SetNumber(number);

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