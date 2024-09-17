using System.Collections.Generic;
using Characters;
using Data;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterPrefabSetting),
        menuName = "Settings/" + nameof(CharacterPrefabSetting))]
    public class CharacterPrefabSetting : ScriptableObject
    {
        [System.Serializable]
        public class PrefabData<TType>
        {
            [SerializeField]
            private TType prefabType;
            [SerializeField]
            private GameObject prefab;
            
            public TType PrefabType => prefabType;

            public GameObject Prefab => prefab;
        }

        [System.Serializable]
        public class HeroPrefabData : PrefabData<HeroType>
        {
        }

        [System.Serializable]
        public class EnemyPrefabData : PrefabData<EnemyType>
        {
        }

        [System.Serializable]
        public class ObstaclePrefabData : PrefabData<ObstacleType>
        {
        }

        [SerializeField] private HeroPrefabData[] heroPrefabDataList;
        [SerializeField] private EnemyPrefabData[] enemyPrefabDataList;
        [SerializeField] private ObstaclePrefabData[] obstaclePrefabDataList;

        public IReadOnlyList<HeroPrefabData> HeroPrefabDataList => heroPrefabDataList;

        public IReadOnlyList<EnemyPrefabData> EnemyPrefabDataList => enemyPrefabDataList;

        public IReadOnlyList<ObstaclePrefabData> ObstaclePrefabDataList => obstaclePrefabDataList;
    }
}