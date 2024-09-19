using UnityEngine;
using UnityEngine.Serialization;

#nullable enable

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(GameSetting), menuName = "Settings/" + nameof(GameSetting))]
    public class GameSetting : ScriptableObject
    {
        [SerializeField] private BoardSetting boardSetting = null!;
        [SerializeField] private CharacterSpawnSetting characterSpawnSetting = null!;
        [SerializeField] private CharacterPrefabSetting characterPrefabSetting = null!;
        [SerializeField] private CharacterDataSetting characterDataSetting = null!;
        [SerializeField] private CharacterDamageCalculationSetting damageCalculationSetting = null!;

        public BoardSetting BoardSetting => boardSetting;

        public CharacterSpawnSetting SpawnSetting => characterSpawnSetting;

        public CharacterPrefabSetting PrefabSetting => characterPrefabSetting;

        public CharacterDataSetting DataSetting => characterDataSetting;

        public CharacterDamageCalculationSetting DamageCalculationSetting => damageCalculationSetting;
    }
}