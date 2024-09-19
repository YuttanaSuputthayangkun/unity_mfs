using System;
using System.Collections.Generic;
using System.Linq;
using Board;
using Characters.Interfaces;
using Data;
using Settings;
using UnityEngine;
using Utilities;

#nullable enable

namespace Characters
{
    public class CharacterSpawnManager
    {
        public struct SpawningResult
        {
            public ICharacter[] SpawnedCharacterList;

            public bool HasSpawned => SpawnedCharacterList?.Length > 0;

            public override string ToString()
            {
                var joinedCharacterString = string.Join("\n", SpawnedCharacterList.Select(x => x.ToString()));
                return $"{nameof(HasSpawned)}\n" +
                       $"{joinedCharacterString}";
            }
        }

        private readonly CharacterSpawnSetting _spawnSetting;
        private readonly CharacterSpawner _spawner;
        private readonly BoardManager _boardManager;
        private readonly NonPlayerCharacterList _nonPlayerCharacterList;

        private WeightedRandom<int>? _weightedRandomSpawnCount;
        private WeightedRandom<CharacterType>? _weightedRandomCharacterType;
        private WeightedRandom<HeroType>? _weightedRandomHeroType;
        private WeightedRandom<EnemyType>? _weightedRandomEnemyType;
        private WeightedRandom<ObstacleType>? _weightedRandomObstacleType;

        private readonly Dictionary<BoardCoordinate, ICharacter> _characterMap = new();

        public CharacterSpawnManager(
            CharacterSpawnSetting spawnSetting,
            CharacterSpawner spawner,
            BoardManager boardManager,
            NonPlayerCharacterList nonPlayerCharacterList
        )
        {
            _spawnSetting = spawnSetting;
            _spawner = spawner;
            _boardManager = boardManager;
            _nonPlayerCharacterList = nonPlayerCharacterList;
        }

        public SpawningResult RandomSpawnOnEmptyCells(CharacterType characterType, int? count = null)
        {
            int emptyCellCount = _boardManager.GetEmptyCellCount();
            int spawnCount = count ?? GetRandomSpawnCount();
            spawnCount = Mathf.Clamp(spawnCount, 0, emptyCellCount);

            var spawnedList = Enumerable.Range(1, spawnCount)
                .Select(
                    _ =>
                    {
                        var spawned = characterType switch
                        {
                            CharacterType.Hero => _spawner.SpawnHero(GetRandomHeroType()) as ICharacter,
                            CharacterType.Enemy => _spawner.SpawnEnemy(GetRandomEnemyType()) as ICharacter,
                            CharacterType.Obstacle => _spawner.SpawnObstacle(GetRandomObstacleType()) as ICharacter,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        return spawned;
                    }
                )
                .Where(x => x is not null)
                .Cast<ICharacter>()
                .ToArray();


            foreach (var spawned in spawnedList)
            {
                var emptyCell = _boardManager.GetRandomEmptyCell();
                if (!emptyCell.IsFound)
                {
                    throw new NotImplementedException(
                        $"we've already checked the empty list count, this should not happen");
                }

                Debug.Log($"get random empty cell: {emptyCell}");

                var placeResult = _boardManager.PlaceCharacter(emptyCell.CellData!.Coordinate, spawned);
                if (!placeResult.IsSuccess)
                {
                    throw new InvalidOperationException(
                        "RandomSpawnOnEmptyCells  failed to place character {placeResult}");
                }

                // update non-player character list
                _nonPlayerCharacterList.Push(spawned);
            }

            return new SpawningResult()
            {
                SpawnedCharacterList = spawnedList,
            };
        }

        public SpawningResult RandomSpawnStartHeroes()
        {
            return RandomSpawnOnEmptyCells(CharacterType.Hero, _spawnSetting.RandomStartHeroSpawnCount);
        }
        
        public SpawningResult RandomSpawnStartEnemies()
        {
            return RandomSpawnOnEmptyCells(CharacterType.Enemy, _spawnSetting.RandomStartEnemySpawnCount);
        }

        public int GetRandomSpawnCount()
        {
            _weightedRandomSpawnCount ??=
                new WeightedRandom<int>(_spawnSetting.CharacterCountDataList.Select(x => (x.Count, x.weight)));

            return _weightedRandomSpawnCount.GetRandomItem();
        }

        public CharacterType GetRandomCharacterType()
        {
            _weightedRandomCharacterType ??=
                new WeightedRandom<CharacterType>(
                    _spawnSetting.CharacterTypeDataList.Select(x => (x.characterType, x.weight)));

            return _weightedRandomCharacterType.GetRandomItem();
        }

        public HeroType GetRandomHeroType()
        {
            _weightedRandomHeroType ??=
                new WeightedRandom<HeroType>(
                    _spawnSetting.HeroTypeDataList.Select(x => (x.heroType, x.weight)));

            return _weightedRandomHeroType.GetRandomItem();
        }

        public EnemyType GetRandomEnemyType()
        {
            _weightedRandomEnemyType ??=
                new WeightedRandom<EnemyType>(
                    _spawnSetting.EnemyTypeDataList.Select(x => (x.enemyType, x.weight)));

            return _weightedRandomEnemyType.GetRandomItem();
        }

        public ObstacleType GetRandomObstacleType()
        {
            _weightedRandomObstacleType ??=
                new WeightedRandom<ObstacleType>(
                    _spawnSetting.ObstacleTypeDataList.Select(x => (x.obstacleType, x.weight)));

            return _weightedRandomObstacleType.GetRandomItem();
        }
    }
}