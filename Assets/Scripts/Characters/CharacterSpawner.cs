using System;
using System.Linq;
using Board;
using Data;
using Settings;
using UnityEditor;
using VContainer;
using VContainer.Unity;

#nullable enable

namespace Characters
{
    public class CharacterSpawner
    {
        public struct CharacterSpawningResult<TBoardObject>
        {
            public TBoardObject? BoardObject;
        }

        private readonly CharacterSpawnSetting _setting;
        private readonly CharacterPrefabSetting _characterPrefabSetting;
        private readonly CharacterDataSetting _characterDataSetting;
        private readonly LifetimeScope _lifetimeScope;

        public CharacterSpawner(
            CharacterSpawnSetting setting,
            CharacterPrefabSetting characterPrefabSetting,
            CharacterDataSetting characterDataSetting,
            LifetimeScope lifetimeScope
        )
        {
            _setting = setting;
            _characterPrefabSetting = characterPrefabSetting;
            _characterDataSetting = characterDataSetting;
            _lifetimeScope = lifetimeScope;
        }

        // public CharacterSpawningResult<TSpawned> SpawnCharacter<TSpawned>(BoardObjectType boardObjectType)
        //     where TSpawned : class
        // {
        //     object newSpawned = boardObjectType switch
        //     {
        //         BoardObjectType.Hero => SpawnHero(),
        //         BoardObjectType.Enemy => SpawnEnemey(),
        //         BoardObjectType.Obstacle => SpawnObstacle(),
        //         _ => throw new ArgumentOutOfRangeException(nameof(boardObjectType), boardObjectType, null)
        //     };
        //     if (newSpawned is not TSpawned)
        //     {
        //         throw new ArgumentException($"Cannot spawn character with type: {typeof(TSpawned).Name}");
        //     }
        //
        //     // TODO: implement this
        //     return new CharacterSpawningResult<TSpawned>()
        //     {
        //         BoardObject = newSpawned as TSpawned,
        //     };
        // }

        public Hero SpawnHero(HeroType heroType)
        {
            // since the list is pretty small, linear search is fine
            var heroData = _characterDataSetting.HeroDataList.First(x => x.Type == heroType)
                           ?? throw new NotSupportedException(heroType.ToString());
            var factory = _lifetimeScope.Container.Resolve<Func<IReadOnlyCharacterData<HeroType>, Hero>>()
                ?? throw new SystemException($"Cannot find hero factory");
            return factory.Invoke(heroData);
        }

        public object SpawnObstacle(ObstacleType obstacleType)
        {
            // since the list is pretty small, linear search is fine
            var prefab = _characterPrefabSetting.ObstaclePrefabDataList.First(x => x.PrefabType == obstacleType)?.Prefab
                         ?? throw new NotSupportedException(obstacleType.ToString());

            using (LifetimeScope.EnqueueParent(_lifetimeScope))
            {
                var newObstacle = _lifetimeScope.Container.Resolve<Obstacle>();
                return newObstacle;
            }
        }

        public Enemy SpawnEnemey(EnemyType enemyType)
        {
            // since the list is pretty small, linear search is fine
            var prefab = _characterPrefabSetting.EnemyPrefabDataList.First(x => x.PrefabType == enemyType)?.Prefab
                         ?? throw new NotSupportedException(enemyType.ToString());

            using (LifetimeScope.EnqueueParent(_lifetimeScope))
            {
                var newEnemy = _lifetimeScope.Container.Resolve<Enemy>();
                return newEnemy;
            }
        }
    }
}