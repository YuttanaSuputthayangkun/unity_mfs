using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Settings;

#nullable enable

namespace Characters
{
    public class CharacterDamageCalculator
    {
        private readonly
            IReadOnlyDictionary<(HeroType, EnemyType), CharacterDamageCalculationSetting.HeroToMonsterMultiplierData>
            heroToMonsterMap;

        private readonly
            IReadOnlyDictionary<(EnemyType, HeroType), CharacterDamageCalculationSetting.MonsterToHeroMultiplierData>
            monsterToHeroMap;

        public struct DamageCalculationResult
        {
        }

        public CharacterDamageCalculator(CharacterDamageCalculationSetting setting)
        {
            heroToMonsterMap = setting.HeroToMonsterMultiplierDataList.ToDictionary(
                x => (x.AttackerType, x.DefenderType),
                x => x
            );
            monsterToHeroMap = setting.MonsterToHeroMultiplierDataList.ToDictionary(
                x => (x.AttackerType, x.DefenderType),
                x => x
            );
        }

        public DamageCalculationResult CalculateDamage()
        {
            throw new NotImplementedException();
        }
    }
}