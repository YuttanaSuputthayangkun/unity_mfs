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

        public BoardSetting BoardSetting => boardSetting;

        public CharacterSpawnSetting SpawnSetting => characterSpawnSetting;
    }
}