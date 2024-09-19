using System;
using System.Linq;
using Data;
using Settings;
using Utilities;
using VContainer;
using VContainer.Unity;

#nullable enable

namespace Characters
{
    public class CharacterSpawner
    {
        private readonly CharacterPrefabSetting _characterPrefabSetting;
        private readonly CharacterDataSetting _characterDataSetting;
        private readonly LifetimeScope _lifetimeScope;

        public CharacterSpawner(
            CharacterPrefabSetting characterPrefabSetting,
            CharacterDataSetting characterDataSetting,
            LifetimeScope lifetimeScope
        )
        {
            _characterPrefabSetting = characterPrefabSetting;
            _characterDataSetting = characterDataSetting;
            _lifetimeScope = lifetimeScope;
        }

        public Hero SpawnHero(HeroType heroType)
        {
            // since the list is pretty small, linear search is fine
            var heroData = _characterDataSetting.HeroDataList.First(x => x.Type == heroType)
                           ?? throw new NotSupportedException(heroType.ToString());
            var factory = _lifetimeScope.Container.Resolve<Func<IReadOnlyCharacterData<HeroType>, Hero>>()
                          ?? throw new SystemException($"Cannot find hero factory");
            return factory.Invoke(heroData);
        }

        public Hero SpawnRandomHero()
        {
            return SpawnHero(EnumUtilities.RandomPickEnum<HeroType>() ??
                             throw new NotSupportedException($"empty hero type"));
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

        public Enemy SpawnEnemy(EnemyType enemyType)
        {
            // since the list is pretty small, linear search is fine
            var enemyData = _characterDataSetting.EnemyDataList.First(x => x.Type == enemyType)
                           ?? throw new NotSupportedException(enemyType.ToString());
            var factory = _lifetimeScope.Container.Resolve<Func<IReadOnlyCharacterData<EnemyType>, Enemy>>()
                          ?? throw new SystemException($"Cannot find enemy factory");
            return factory.Invoke(enemyData);
        }

        public Enemy RandomSpawnEnemy()
        {
            return SpawnEnemy(EnumUtilities.RandomPickEnum<EnemyType>() ??
                             throw new NotSupportedException($"empty enemy type"));
        }
    }
}