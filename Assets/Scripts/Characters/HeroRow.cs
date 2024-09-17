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

    public class HeroRow :
        IContainReadOnlyRowHeroDataList
    {
        private readonly BoardManager _boardManager;
        private readonly BoardSetting _boardSetting;
        private readonly CharacterSpawner _spawner;
        private readonly CharacterDataSetting _characterDataSetting;

        private readonly Queue<RowHeroData> _characterQueue = new Queue<RowHeroData>();

        public HeroRow(BoardManager boardManager, BoardSetting boardSetting, CharacterSpawner spawner,
            CharacterDataSetting characterDataSetting)
        {
            _boardManager = boardManager;
            _boardSetting = boardSetting;
            _spawner = spawner;
            _characterDataSetting = characterDataSetting;
        }

        public IEnumerable<IReadOnlyRowHeroData> RowHeroDataList => _characterQueue;

        private RowHeroData? Head => _characterQueue.Peek();

        public override string ToString()
        {
            var characterStrings = _characterQueue.Select((x, i) => $"[{i}]{x}");
            return string.Join("\n", characterStrings);
        }

        // public void SetupStartHero(BoardCoordinate boardCoordinate, IReadOnlyCharacterData<HeroType> readOnlyHeroData)
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
            _characterQueue.Enqueue(newRowData);
        }

        public bool TryMove(Direction direction)
        {
            // check from head if the direction is movable 
            var head = Head!;
            var headCoordinate = head.Coordinate;
            var nextHeadCoordinate = headCoordinate.GetNeighbor(direction);

            var getCellResult = _boardManager.GetCell(nextHeadCoordinate);
            if (getCellResult is { IsFound: true, CellData: { IsOccupied: false } })
            {
                // call direction, aside from the first one
                var pairs = _characterQueue.ToPairsWithPreviousClass();
                foreach (var (previousHero, hero) in pairs)
                {
                    if (previousHero is RowHeroData previous)
                    {
                        // means this is not the head of the row
                        // move to previous hero's position
                        var nextCoordinate = previous.Coordinate;
                        if (!TryMoveHero(nextCoordinate, hero))
                        {
                            // TODO: use a proper exception type
                            throw new Exception("this should not happen, following hero should always be able to move");
                        }
                    }
                    else
                    {
                        // this is the head, go to next head coordinate
                        if (!TryMoveHero(nextHeadCoordinate, hero))
                        {
                            // TODO: use a proper exception type
                            throw new Exception("this should not happen, we've already checked head coordinate");
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryMoveHero(BoardCoordinate nextBoardCoordinate, RowHeroData rowHeroData)
        {
            var currentCoordinate = rowHeroData.Coordinate;
            var getCellResult = _boardManager.GetCell(nextBoardCoordinate);
            if (getCellResult is { IsFound: true, CellData: { IsOccupied: false, WorldPosition: var nextWorldPosition } })
            {
                _boardManager.SetCellObjectType(nextBoardCoordinate, rowHeroData.Hero.BoardObjectType);
                rowHeroData.UpdateCoordinate(nextBoardCoordinate);
                rowHeroData.Hero.SetWorldPosition(nextWorldPosition);

                _boardManager.SetCellObjectType(currentCoordinate, null);
                
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}