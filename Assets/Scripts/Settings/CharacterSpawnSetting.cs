using System;
using System.Collections.Generic;
using Characters;
using Data;
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

        public IReadOnlyList<CharacterTypeSpawningData> CharacterTypeSpawningDataList => characterTypeSpawningDataList;

        public IReadOnlyList<HeroSpawningChanceData> HeroSpawningChanceDataList => heroSpawningChanceDataList;
    }
}