using System;
using System.Collections.Generic;
using Board;
using System.Linq;
using Data;
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

        public HeroRow(BoardManager boardManager, BoardSetting boardSetting, CharacterSpawner spawner, CharacterDataSetting characterDataSetting)
        {
            _boardManager = boardManager;
            _boardSetting = boardSetting;
            _spawner = spawner;
            _characterDataSetting = characterDataSetting;
        }

        public IEnumerable<IReadOnlyRowHeroData> RowHeroDataList => _characterQueue;

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
            var heroData = _characterDataSetting.HeroDataList.First() ?? throw new NotSupportedException($"No data of hero with type: {heroType}");
            
            var getCellResult = _boardManager.GetCell(boardCoordinate);
            if (!getCellResult.IsFound)
            {
                throw new IndexOutOfRangeException($"SetupStartHero can't find cell at ({boardCoordinate})");
            }

            // this pattern match also checks for null
            if (getCellResult.CellData is { IsOccupied: true})
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
            throw new NotImplementedException();
        }
    }
}