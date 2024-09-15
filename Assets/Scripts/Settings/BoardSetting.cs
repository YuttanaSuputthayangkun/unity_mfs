#nullable enable

using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(BoardSetting), menuName = "Settings/" + nameof(BoardSetting))]
    public class BoardSetting : ScriptableObject
    {
        [Range(0, 100)] [SerializeField] private int boardHeight;
        [Range(0, 100)] [SerializeField] private int boardWidth;

        public int BoardHeight => boardHeight;

        public int BoardWidth => boardWidth;
    }
}