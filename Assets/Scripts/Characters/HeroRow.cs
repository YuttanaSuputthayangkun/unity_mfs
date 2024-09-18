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
        private readonly CharacterSpawner _spawner;
        private readonly CharacterDataSetting _characterDataSetting;

        private readonly HeroList _heroList = new();

        public HeroRow(
            BoardManager boardManager,
            BoardSetting boardSetting,
            CharacterSpawner spawner,
            CharacterDataSetting characterDataSetting
        )
        {
            _boardManager = boardManager;
            _boardSetting = boardSetting;
            _spawner = spawner;
            _characterDataSetting = characterDataSetting;
        }

        public IEnumerable<IReadOnlyCharacter> ReadOnlyHeroList => _heroList.RowHeroDataList;

        public int HeroCount => _heroList.Count;

        public Hero? GetFirst() => _heroList.GetFirst();
        
        public Hero? GetLast() => _heroList.GetLast();

        public override string ToString() => _heroList.ToString();

        public void SetupStartHero()
        {
            var boardCoordinate = _boardSetting.StartHeroCoordinate;
            var heroType = _boardSetting.StartHeroType;
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
            // spawnedHero.SetWorldPosition(getCellResult.CellData!.WorldPosition); // let the pool do the work

            // update board cell type
            _boardManager.SetCellCharacter(boardCoordinate, spawnedHero);

            _heroList.Add(spawnedHero);
        }

        public bool ContainsHero(Hero hero) => _heroList.ContainsHero(hero);

        public bool IsLastHero(Hero hero) => _heroList.GetLast() == hero;

        public bool AddLast(BoardCoordinate coordinate)
        {
            var last = GetLast();
            if (last is null)
            {
                // row is empty
                return false;
            }
            
            // ensure added coordinate is a neighbor to the last
            // bool isNeighborToLast = last.GetBoardCoordinate().IsNeighbor(coordinate);
            // if (!isNeighborToLast)
            // {
            //     throw new ArgumentException("cannot add");
            // }

            throw new NotImplementedException();
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
            var pairs = _heroList.RowHeroDataList.ToPairsWithPreviousClass();
            foreach (var (previousHero, hero) in pairs)
            {
                if (previousHero is Hero previous)
                {
                    // means this is not the head of the row
                    // move to previous hero's position
                    var nextCoordinate = previous.GetBoardCoordinate()!.Value;
                    if (hero.TryMove(nextCoordinate) != MoveResultType.Success)
                    {
                        // TODO: use a proper exception type
                        throw new Exception("this should not happen, following hero should always be able to move");
                    }
                }
                else
                {
                    // this is the head, go to next head coordinate
                    if (hero.TryMove(nextHeadCoordinate) != MoveResultType.Success)
                    {
                        // TODO: use a proper exception type
                        throw new Exception("this should not happen, we've already checked head coordinate");
                    }
                }
            }

            return MoveResultType.Success;
        }
    }
}