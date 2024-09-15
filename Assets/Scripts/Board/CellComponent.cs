using UnityEngine;

#nullable enable

namespace Board
{
    public class CellComponent : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer = null!;

        public SpriteRenderer SpriteRenderer => spriteRenderer;
    }
}