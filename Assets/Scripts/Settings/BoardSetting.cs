#nullable enable

using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(BoardSetting), menuName = "Settings/" + nameof(BoardSetting))]
    public class BoardSetting : ScriptableObject
    {
        [SerializeField] private uint boardHeight;
        [SerializeField] private uint boardWidth;

        public uint BoardHeight => boardHeight;

        public uint BoardWidth => boardWidth;
    }
}