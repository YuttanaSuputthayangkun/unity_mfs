using System.Collections.Generic;
using Characters;
using Data;
using UnityEngine;

#nullable enable

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterDataSetting), menuName = "Settings/" + nameof(CharacterDataSetting))]
    public class CharacterDataSetting : ScriptableObject
    {
        [SerializeField] private HeroData[] heroDataList = null!;
        [SerializeField] private EnemyData[] enemyDataList = null!;
        [SerializeField] private ObstacleData[] obstacleDataList = null!;

        // public IReadOnlyList<HeroData> HeroDataList => heroDataList;
        public IReadOnlyList<IReadOnlyCharacterData<HeroType>> HeroDataList => heroDataList;

        public IReadOnlyList<EnemyData> EnemyDataList => enemyDataList;

        public IReadOnlyList<ObstacleData> ObstacleDataList => obstacleDataList;
    }
}