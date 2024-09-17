using System.Collections.Generic;
using Characters;
using Data;
using UnityEngine;

#nullable enable

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterPrefabSetting),
        menuName = "Settings/" + nameof(CharacterPrefabSetting))]
    public class CharacterPrefabSetting : ScriptableObject
    {
        [System.Serializable]
        public class PrefabData<TType> where TType : struct
        {
            [SerializeField]
            private TType prefabType;
            [SerializeField]
            private GameObject prefab = null!;
            
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

        [SerializeField] private HeroPrefabData[] heroPrefabDataList = null!;
        [SerializeField] private EnemyPrefabData[] enemyPrefabDataList = null!;
        [SerializeField] private ObstaclePrefabData[] obstaclePrefabDataList = null!;

        public IReadOnlyList<HeroPrefabData> HeroPrefabDataList => heroPrefabDataList;

        public IReadOnlyList<EnemyPrefabData> EnemyPrefabDataList => enemyPrefabDataList;

        public IReadOnlyList<ObstaclePrefabData> ObstaclePrefabDataList => obstaclePrefabDataList;
    }
}