using System;
using System.Collections.Generic;
using Board;
using System.Linq;
using Characters.Interfaces;
using Data;
using Extensions;
using Settings;
using UnityEngine;

#nullable enable

namespace Characters
{
    public partial class HeroRow
    {
        private readonly BoardManager _boardManager;
        private readonly BoardSetting _boardSetting;
        private readonly CharacterSpawnSetting _characterSpawnSetting;
        private readonly CharacterSpawner _spawner;
        private readonly CharacterDataSetting _characterDataSetting;

        private readonly HeroList _heroList = new();

        public HeroRow(
            BoardManager boardManager,
            BoardSetting boardSetting,
            CharacterSpawnSetting characterSpawnSetting,
            CharacterSpawner spawner,
            CharacterDataSetting characterDataSetting
        )
        {
            _boardManager = boardManager;
            _boardSetting = boardSetting;
            _characterSpawnSetting = characterSpawnSetting;
            _spawner = spawner;
            _characterDataSetting = characterDataSetting;
        }

        public IEnumerable<IReadOnlyCharacter> ReadOnlyHeroList => _heroList.RowHeroDataList;

        public int HeroCount => _heroList.Count;

        public Hero? GetFirst() => _heroList.GetFirst();

        public Hero? GetLast() => _heroList.GetLast();

        public int? GetHeroIndex(Hero hero) => _heroList.GetHeroIndex(hero);

        public override string ToString() => _heroList.ToString();

        public void SetupStartHero()
        {
            var boardCoordinate = _boardSetting.StartHeroCoordinate;
            var heroType = _characterSpawnSetting.StartHeroType;
            var heroData = _characterDataSetting.HeroDataList.First(x => x.Type == heroType)
                           ?? throw new NotSupportedException($"No data of hero with type: {heroType}");

            var getCellResult = _boardManager.GetCell(boardCoordinate);
            if (!getCellResult.IsFound)
            {
                throw new IndexOutOfRangeException($"SetupStartHero can't find cell at ({boardCoordinate})");
            }

            if (getCellResult.CellData?.Character is not null)
            {
                // TODO: use a proper exception type
                throw new Exception($"SetupStartHero cell ({boardCoordinate}) is already occupied");
            }

            // create and setup new hero
            var spawnedHero = _spawner.SpawnHero(heroType);
            spawnedHero.SetNumber(1);

            // update board cell type
            var placeResult = _boardManager.PlaceCharacter(boardCoordinate, spawnedHero);
            if (!placeResult.IsSuccess)
            {
                throw new InvalidOperationException("SetupStartHero failed to place character {placeResult}");
            }

            _heroList.Add(spawnedHero);
        }

        public bool ContainsHero(Hero hero) => _heroList.ContainsHero(hero);

        public bool IsLastHero(Hero hero) => _heroList.GetLast() == hero;

        public bool AddLast(BoardCoordinate coordinate, Hero hero)
        {
            var last = GetLast();
            if (last is null)
            {
                // row is empty, cannot happen
                throw new InvalidOperationException();
            }

            var getResult = _boardManager.GetCell(coordinate);
            if (!getResult.IsFound)
            {
                throw new ArgumentException("invalid coordinate, no cell");
            }

            var lastHeroCoordinate = last.GetBoardCoordinate()!.Value;
            if (
                !lastHeroCoordinate.IsNeighbor(coordinate)
                && !lastHeroCoordinate.Equals(coordinate) // this is fine, it's when there's only one hero
            )
            {
                throw new ArgumentException("cannot add cell that are not neighbor to the last hero\n" +
                                            $"last: {last.GetBoardCoordinate()}\n" +
                                            $"coordinate: {coordinate}");
            }

            var placeResult = _boardManager.PlaceCharacter(coordinate, hero);
            if (!placeResult.IsSuccess)
            {
                throw new InvalidOperationException($"AddLast {placeResult}");
            }

            var newNumber = _heroList.Count + 1;
            hero.SetNumber(newNumber);

            _heroList.Add(hero);

            return true;
        }

        public Hero RemoveFirst()
        {
            var removed = _heroList.RemoveFirst();

            // re-assign numbers
            var list = _heroList.RowHeroDataList.Select((h, i) => (h, i));
            foreach (var (h, i) in list)
            {
                h.SetNumber(i + 1);
            }

            return removed;
        }

        public MoveResultType TryMove(Direction direction)
        {
            // check from head if the direction is movable 
            var head = GetFirst()!;
            var headCoordinate = head.GetBoardCoordinate();
            var nextHeadCoordinate = headCoordinate!.Value.GetNeighbor(direction);

            var getCellResult = _boardManager.GetCell(nextHeadCoordinate);
            if (!getCellResult.IsFound)
            {
                return MoveResultType.OutOfBound;
            }

            if (getCellResult.CellData?.Character is not null)
            {
                return MoveResultType.OccupiedByOtherObject;
            }

            // call direction, aside from the first one

            var heroPairs = _heroList.RowHeroDataList.ToPairsWithPreviousClass();
            var nextCoordinateHeroPairs = heroPairs.Select(
                x =>
                {
                    var (previousHero, hero) = x;
                    BoardCoordinate? nextCoordinate = previousHero?.GetBoardCoordinate();
                    return (nextCoordinate, hero);
                }
            ).ToArray(); // ToArray to avoid multiple iteration
            {
                // debug move list
                // var strings = nextCoordinateHeroPairs.Select(x => $"{x.nextCoordinate} {x.hero}");
                // var joined = string.Join("\n", strings);
                // Debug.Log($"nextCoordinateHeroPairs: \n{joined}");
            }
            foreach (var (previousHeroCoordinate, hero) in nextCoordinateHeroPairs)
            {
                if (previousHeroCoordinate.HasValue)
                {
                    // means this is not the head of the row
                    // move to previous hero's position
                    var moveResult = hero.TryMove(previousHeroCoordinate.Value);
                    if (moveResult != MoveResultType.Success)
                    {
                        throw new InvalidOperationException(
                            $"this should not happen, following hero should always be able to move\n" +
                            $"previousHeroCoordinate: {previousHeroCoordinate.Value}\n" +
                            $"move result:{moveResult}");
                    }
                }
                else
                {
                    // this is the head, go to next head coordinate
                    var moveResult = hero.TryMove(nextHeadCoordinate);
                    Debug.Log($"TryMove head moveResult: {moveResult}");
                    Debug.Log($"TryMove head board: {_boardManager.GetDebugText()}");
                    if (moveResult != MoveResultType.Success)
                    {
                        throw new InvalidOperationException(
                            "this should not happen, we've already checked head coordinate");
                    }
                }
            }

            return MoveResultType.Success;
        }
    }
}