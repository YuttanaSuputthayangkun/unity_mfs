using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Settings;
using UnityEngine;

#nullable enable

namespace Characters
{
    public class CharacterDamageCalculator
    {
        public struct DamageCalculationResult
        {
            public int Damage;
        }

        private const float BaseMultiplier = 1;

        private readonly
            IReadOnlyDictionary<(HeroType, EnemyType), CharacterDamageCalculationSetting.HeroToEnemyMultiplierData>
            heroToEnemyMap;

        private readonly
            IReadOnlyDictionary<(EnemyType, HeroType), CharacterDamageCalculationSetting.EnemyToHeroMultiplierData>
            enemyToHeroMap;

        public CharacterDamageCalculator(CharacterDamageCalculationSetting setting)
        {
            heroToEnemyMap = setting.HeroToEnemyMultiplierDataList.ToDictionary(
                x => (x.AttackerType, x.DefenderType),
                x => x
            );
            enemyToHeroMap = setting.EnemyToHeroMultiplierDataList.ToDictionary(
                x => (x.AttackerType, x.DefenderType),
                x => x
            );
        }

        public DamageCalculationResult CalculateDamage<TAttackerType, TDefenderType>(
            ICharacterStats attacker,
            TAttackerType attackerType,
            ICharacterStats defender,
            TDefenderType defenderType
        )
        {
            float multiplier = GetMultiplier(attackerType, defenderType);
            var attack = Mathf.CeilToInt(attacker.Attack * multiplier);
            int damage = attack - defender.Defense;
            damage = Mathf.Clamp(damage, 0, int.MaxValue);
            
            Debug.Log($"CalculateDamage ({attacker.Attack}-{defender.Defense}) = {damage}");

            return new DamageCalculationResult()
            {
                Damage = damage,
            };
        }

        private float GetMultiplier<TAttackerType, TDefenderType>(
            TAttackerType attackerType,
            TDefenderType defenderType
        )
        {
            switch ((attackerType, defenderType))
            {
                case (HeroType heroType, EnemyType enemyType):
                {
                    return heroToEnemyMap.TryGetValue((heroType, enemyType), out var multiplierData)
                            ? multiplierData.Multiplier
                            : BaseMultiplier
                        ;
                }
                case (EnemyType enemyType, HeroType heroType):
                {
                    return enemyToHeroMap.TryGetValue((enemyType, heroType), out var multiplierData)
                            ? multiplierData.Multiplier
                            : BaseMultiplier
                        ;
                }
                default:
                    throw new NotSupportedException($"{typeof(TAttackerType).Name} {typeof(TDefenderType).Name}");
            }
        }
    }
}