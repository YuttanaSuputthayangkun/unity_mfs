using UnityEngine;

#nullable enable

namespace Board
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer = null!;

        public SpriteRenderer SpriteRenderer => spriteRenderer;
    }
}