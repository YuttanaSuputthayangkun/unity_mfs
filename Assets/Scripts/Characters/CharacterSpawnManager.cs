using System;
using System.Linq;
using Board;
using Characters.Interfaces;
using Data;
using Settings;
using Utilities;

#nullable enable

namespace Characters
{
    public class CharacterSpawnManager
    {
        public struct SpawnedCharacterData
        {
            public BoardCoordinate BoardCoordinate;
            public object SpawnedCharacter;

            public override string ToString()
            {
                return $"Coordinate({BoardCoordinate}) Character({SpawnedCharacter.GetHashCode()})";
            }
        }

        public struct SpawningResult
        {
            public SpawnedCharacterData[] SpawnedCharacterDataList;

            public bool HasSpawned => SpawnedCharacterDataList?.Length > 0;

            public override string ToString()
            {
                var joinedCharacterString = string.Join("\n", SpawnedCharacterDataList.Select(x => x.ToString()));
                return $"{nameof(HasSpawned)}\n" +
                       $"{joinedCharacterString}";
            }
        }

        private readonly CharacterSpawnSetting _spawnSetting;
        private readonly CharacterSpawner _spawner;
        private readonly BoardManager _boardManager;

        private WeightedRandom<int>? _weightedRandomSpawnCount;
        private WeightedRandom<CharacterType>? _weightedRandomCharacterType;
        private WeightedRandom<HeroType>? _weightedRandomHeroType;
        private WeightedRandom<EnemyType>? _weightedRandomEnemyType;
        private WeightedRandom<ObstacleType>? _weightedRandomObstacleType;

        public CharacterSpawnManager(
            CharacterSpawnSetting spawnSetting,
            CharacterSpawner spawner,
            BoardManager boardManager
        )
        {
            _spawnSetting = spawnSetting;
            _spawner = spawner;
            _boardManager = boardManager;
        }

        public SpawningResult RandomSpawn()
        {
            int spawnCount = GetRandomSpawnCount();

            var spawnedDataList = Enumerable.Range(1, spawnCount).Select(
                x =>
                {
                    var emptyCell = _boardManager.GetRandomEmptyCell();
                    if (!emptyCell.IsFound)
                    {
                        // out of empty cells
                        return null;
                    }

                    var characterType = GetRandomCharacterType();
                    var spawned = characterType switch
                    {
                        CharacterType.Hero => _spawner.SpawnHero(GetRandomHeroType()),
                        CharacterType.Enemy => _spawner.SpawnEnemy(GetRandomEnemyType()),
                        CharacterType.Obstacle => _spawner.SpawnObstacle(GetRandomObstacleType()),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (spawned is ISetWorldPosition setWorldPosition)
                    {
                        setWorldPosition.SetWorldPosition(emptyCell.CellData!.WorldPosition);
                    }
                    else
                    {
                        throw new NotSupportedException(
                            $"make sure to implement {nameof(ISetWorldPosition)} to spawned character");
                    }

                    return new SpawnedCharacterData()
                    {
                        BoardCoordinate = emptyCell.CellData!.Coordinate,
                        SpawnedCharacter = spawned,
                    } as SpawnedCharacterData?;
                }
            ).Where(x => x is not null).Cast<SpawnedCharacterData>().ToArray();

            return new SpawningResult()
            {
                SpawnedCharacterDataList = spawnedDataList,
            };
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