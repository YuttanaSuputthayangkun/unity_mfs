#nullable enable

using Board;
using Characters;
using Data;
using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = nameof(BoardSetting), menuName = "Settings/" + nameof(BoardSetting))]
    public class BoardSetting : ScriptableObject
    {
        [Range(0, 100)] [SerializeField] private int boardHeight;
        [Range(0, 100)] [SerializeField] private int boardWidth;
        [SerializeField] private CellComponent cellComponentPrefab = null!;
        [SerializeField] private BoardCoordinate startHeroCoordinate = new BoardCoordinate(8, 8);
        [SerializeField] private HeroType startHeroType = HeroType.Rogue;

        public int BoardHeight => boardHeight;

        public int BoardWidth => boardWidth;

        public CellComponent CellComponentPrefab => cellComponentPrefab;

        public BoardCoordinate StartHeroCoordinate => startHeroCoordinate;

        public HeroType StartHeroType => startHeroType;
    }
}