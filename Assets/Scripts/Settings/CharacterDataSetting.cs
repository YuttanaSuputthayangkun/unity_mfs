using System;
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

        public HeroData[] HeroDataList => heroDataList;

        public EnemyData[] EnemyDataList => enemyDataList;

        public ObstacleData[] ObstacleDataList => obstacleDataList;
    }
}