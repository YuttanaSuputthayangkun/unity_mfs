using UnityEngine;
using UnityEngine.Serialization;

#nullable enable

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(GameSetting), menuName = "Settings/" + nameof(GameSetting))]
    public class GameSetting : ScriptableObject
    {
        [SerializeField] private BoardSetting boardSetting = null!;

        public BoardSetting BoardSetting => boardSetting;
    }
}