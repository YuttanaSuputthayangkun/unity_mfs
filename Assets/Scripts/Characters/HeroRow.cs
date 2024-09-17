using System;
using System.Collections.Generic;
using Board;
using System.Linq;
using Data;
using Extensions;
using Settings;
using UnityEngine;

#nullable enable

namespace Characters
{
    public interface IContainReadOnlyRowHeroDataList
    {
        IEnumerable<IReadOnlyRowHeroData> RowHeroDataList { get; }
    }

    public partial class HeroRow :
        IContainReadOnlyRowHeroDataList
    {
        private readonly BoardManager _boardManager;
        private readonly BoardSetting _boardSetting;
        private readonly CharacterSpawner _spawner;
        private readonly CharacterDataSetting _characterDataSetting;

        private readonly HeroList _heroList = new();

        public HeroRow(BoardManager boardManager, BoardSetting boardSetting, CharacterSpawner spawner,
            CharacterDataSetting characterDataSetting)
        {
            _boardManager = boardManager;
            _boardSetting = boardSetting;
            _spawner = spawner;
            _characterDataSetting = characterDataSetting;
        }

        public IEnumerable<IReadOnlyRowHeroData> RowHeroDataList => _heroList.RowHeroDataList;

        public int HeroCount => _heroList.Count;

        private IReadOnlyRowHeroData? First => _heroList.GetFirst();

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

            // this pattern match also checks for null
            if (getCellResult.CellData is { IsOccupied: true })
            {
                // TODO: use a proper exception type
                throw new Exception($"SetupStartHero cell ({boardCoordinate}) is already occupied");
            }

            // spawn
            var spawnedHero = _spawner.SpawnHero(heroType);

            // set position
            spawnedHero.SetWorldPosition(getCellResult.CellData!.WorldPosition);

            // update board cell type
            _boardManager.SetCellObjectType(boardCoordinate, BoardObjectType.Hero);

            // store line information
            var newHeroData = new HeroData(heroData);
            var newRowData = new RowHeroData(boardCoordinate, newHeroData, spawnedHero);
            _heroList.Add(newRowData);
        }

        public bool ContainsHero(Hero hero) => _heroList.ContainsHero(hero);

        public bool IsLastHero(Hero hero) => _heroList.GetLast()?.Hero == hero;

        public MoveResultType TryMove(Direction direction)
        {
            // check from head if the direction is movable 
            var head = First!;
            var headCoordinate = head.Coordinate;
            var nextHeadCoordinate = headCoordinate.GetNeighbor(direction);

            var getCellResult = _boardManager.GetCell(nextHeadCoordinate);
            if (!getCellResult.IsFound)
            {
                return MoveResultType.OutOfBound;
            }

            if (getCellResult.CellData!.IsOccupied)
            {
                return MoveResultType.OccupiedByOtherObject;
            }

            // call direction, aside from the first one
            var pairs = _heroList.RowHeroDataList.ToPairsWithPreviousClass();
            foreach (var (previousHero, hero) in pairs)
            {
                if (previousHero is RowHeroData previous)
                {
                    // means this is not the head of the row
                    // move to previous hero's position
                    var nextCoordinate = previous.Coordinate;
                    if (TryMoveHero(nextCoordinate, hero) != MoveResultType.Success)
                    {
                        // TODO: use a proper exception type
                        throw new Exception("this should not happen, following hero should always be able to move");
                    }
                }
                else
                {
                    // this is the head, go to next head coordinate
                    if (TryMoveHero(nextHeadCoordinate, hero) != MoveResultType.Success)
                    {
                        // TODO: use a proper exception type
                        throw new Exception("this should not happen, we've already checked head coordinate");
                    }
                }
            }

            return MoveResultType.Success;
        }

        private MoveResultType TryMoveHero(BoardCoordinate nextBoardCoordinate, RowHeroData rowHeroData)
        {
            var currentCoordinate = rowHeroData.Coordinate;
            var getCellResult = _boardManager.GetCell(nextBoardCoordinate);
            if (!getCellResult.IsFound)
            {
                return MoveResultType.OutOfBound;
            }

            if (getCellResult.CellData!.IsOccupied)
            {
                return MoveResultType.OccupiedByOtherObject;
            }

            var nextWorldPosition = getCellResult.CellData!.WorldPosition;
            _boardManager.SetCellObjectType(nextBoardCoordinate, rowHeroData.Hero.BoardObjectType);
            rowHeroData.UpdateCoordinate(nextBoardCoordinate);
            rowHeroData.Hero.SetWorldPosition(nextWorldPosition);

            _boardManager.SetCellObjectType(currentCoordinate, null);

            return MoveResultType.Success;
        }
    }
}