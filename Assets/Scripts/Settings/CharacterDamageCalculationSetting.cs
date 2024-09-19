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
        public class HeroToMonsterMultiplierData : CharacterDamageMultiplierData<HeroType, EnemyType>
        {
        }

        [System.Serializable]
        public class MonsterToHeroMultiplierData : CharacterDamageMultiplierData<EnemyType, HeroType>
        {
        }

        [SerializeField] private HeroToMonsterMultiplierData[] heroToMonsterMultiplierDataList = null!;

        [SerializeField] private MonsterToHeroMultiplierData[] monsterToHeroMultiplierDataList = null!;

        public IReadOnlyList<HeroToMonsterMultiplierData> HeroToMonsterMultiplierDataList =>
            heroToMonsterMultiplierDataList;

        public IReadOnlyList<MonsterToHeroMultiplierData> MonsterToHeroMultiplierDataList =>
            monsterToHeroMultiplierDataList;
    }
}