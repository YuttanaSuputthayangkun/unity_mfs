using System;
using System.Collections.Generic;
using Characters;
using Data;
using UnityEngine;
using UnityEngine.Serialization;

#nullable enable

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterSpawnSetting), menuName = "Settings/" + nameof(CharacterSpawnSetting))]
    public class CharacterSpawnSetting : ScriptableObject
    {
        [System.Serializable]
        public struct CharacterTypeData
        {
            public CharacterType characterType;
            public int weight;
        }

        [System.Serializable]
        public struct HeroTypeData
        {
            public HeroType heroType;
            public int weight;
        }
        
        [System.Serializable]
        public struct EnemyTypeData
        {
            public EnemyType enemyType;
            public int weight;
        }
        
        [System.Serializable]
        public struct ObstacleTypeData
        {
            public ObstacleType obstacleType;
            public int weight;
        }

        [System.Serializable]
        public struct CharacterCountData
        {
            public int Count;
            public int weight;
        }

        [SerializeField] private CharacterTypeData[] characterTypeDataList = null!;
        [SerializeField] private HeroTypeData[] heroTypeDataList = null!;
        [SerializeField] private EnemyTypeData[] enemyTypeDataList = null!;
        [SerializeField] private ObstacleTypeData[] _obstacleTypeDataList = null!;
        [SerializeField] private CharacterCountData[] characterCountDataList = null!;

        public IReadOnlyList<CharacterTypeData> CharacterTypeDataList => characterTypeDataList;

        public IReadOnlyList<HeroTypeData> HeroTypeDataList => heroTypeDataList;

        public IReadOnlyList<EnemyTypeData> EnemyTypeDataList => enemyTypeDataList;

        public IReadOnlyList<CharacterCountData> CharacterCountDataList => characterCountDataList;

        public IReadOnlyList<ObstacleTypeData> ObstacleTypeDataList => _obstacleTypeDataList;
    }
}