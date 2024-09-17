using System;
using System.Linq;
using Board;
using Characters.Interfaces;
using Data;
using Extensions;
using Settings;

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
            var data = _spawnSetting.CharacterCountDataList.RandomPickStruct() ??
                       throw new NotSupportedException("spawn count is not set");
            return data.Count;
        }

        public CharacterType GetRandomCharacterType()
        {
            var data = _spawnSetting.CharacterTypeDataList.RandomPickStruct() ??
                       throw new NotSupportedException("spawn count is not set");
            return data.characterType;
        }

        public HeroType GetRandomHeroType()
        {
            var data = _spawnSetting.HeroTypeDataList.RandomPickStruct() ??
                       throw new NotSupportedException("spawn count is not set");
            return data.heroType;
        }

        public EnemyType GetRandomEnemyType()
        {
            var data = _spawnSetting.EnemyTypeDataList.RandomPickStruct() ??
                       throw new NotSupportedException("spawn count is not set");
            return data.enemyType;
        }

        public ObstacleType GetRandomObstacleType()
        {
            var data = _spawnSetting.ObstacleTypeDataList.RandomPickStruct() ??
                       throw new NotSupportedException("spawn count is not set");
            return data.obstacleType;
        }
    }
}