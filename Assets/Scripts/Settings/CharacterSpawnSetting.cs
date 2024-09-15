using System;
using Characters;
using UnityEngine;

#nullable enable

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterSpawnSetting), menuName = "Settings/" + nameof(CharacterSpawnSetting))]
    public class CharacterSpawnSetting : ScriptableObject
    {
        [System.Serializable]
        public struct CharacterTypeSpawningData
        {
            public CharacterType characterType;
            public int weight;
        }

        [System.Serializable]
        public struct HeroSpawningChanceData
        {
            public HeroType heroType;
            public int weight;
        }

        [SerializeField] private CharacterTypeSpawningData[] characterTypeSpawningDataList = null!;
        [SerializeField] private HeroSpawningChanceData[] heroSpawningChanceDataList = null!;

        public CharacterTypeSpawningData[] CharacterTypeSpawningDataList => characterTypeSpawningDataList;

        public HeroSpawningChanceData[] HeroSpawningChanceDataList => heroSpawningChanceDataList;
    }
}