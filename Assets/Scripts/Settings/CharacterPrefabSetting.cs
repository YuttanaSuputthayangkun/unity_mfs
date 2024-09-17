using Characters;
using Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterPrefabSetting),
        menuName = "Settings/" + nameof(CharacterPrefabSetting))]
    public class CharacterPrefabSetting : ScriptableObject
    {
        [System.Serializable]
        public class PrefabData<TType>
        {
            public TType PrefabType;
            public GameObject Prefab;
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

        public HeroPrefabData[] HeroPrefabDataList => heroPrefabDataList;

        public EnemyPrefabData[] EnemyPrefabDataList => enemyPrefabDataList;

        public ObstaclePrefabData[] ObstaclePrefabDataList => obstaclePrefabDataList;
    }
}