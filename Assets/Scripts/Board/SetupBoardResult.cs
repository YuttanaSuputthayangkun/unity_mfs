using System;
using Data;
using UnityEngine;

#nullable enable

namespace Board
{
    public partial class BoardManager
    {
        public struct SetupBoardResult
        {
            public Vector2 BoardPosition;
            public Vector2 BoardSize;

            public override string ToString()
            {
                return $"{nameof(BoardPosition)}({BoardPosition})" +
                       $"{nameof(BoardSize)}({BoardSize})";
            }
        }
    }
}
