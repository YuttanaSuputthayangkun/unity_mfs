using System.Collections.Generic;
using Characters;
using Data;
using UnityEngine;

#nullable enable

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(CharacterDamageCalculationSetting),
        menuName = "Settings/" + nameof(CharacterDamageCalculationSetting))]
    public class CharacterDamageCalculationSetting : ScriptableObject
    {
        public class CharacterDamageMultiplierData<TAttackerType, TDefenderType>
            where TAttackerType : struct
            where TDefenderType : struct
        {
            public TAttackerType AttackerType;
            public TDefenderType DefenderType;
            public float Multiplier;
        }

        [System.Serializable]
        public class HeroToEnemyMultiplierData : CharacterDamageMultiplierData<HeroType, EnemyType>
        {
        }

        [System.Serializable]
        public class EnemyToHeroMultiplierData : CharacterDamageMultiplierData<EnemyType, HeroType>
        {
        }

        [SerializeField] private HeroToEnemyMultiplierData[] heroToEnemyMultiplierDataList = null!;

        [SerializeField] private EnemyToHeroMultiplierData[] enemyToHeroMultiplierDataList = null!;

        public IReadOnlyList<HeroToEnemyMultiplierData> HeroToEnemyMultiplierDataList =>
            heroToEnemyMultiplierDataList;

        public IReadOnlyList<EnemyToHeroMultiplierData> EnemyToHeroMultiplierDataList =>
            enemyToHeroMultiplierDataList;
    }
}