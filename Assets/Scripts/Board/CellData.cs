using System;
using Characters.Interfaces;
using Data;
using UnityEngine;

#nullable enable

namespace Board
{
    public partial class BoardManager
    {
        public interface IReadOnlyCellData
        {
            BoardCoordinate Coordinate { get; }
            ICharacter? Character { get; }
            Vector3 WorldPosition { get; }
        }

        private class CellData : IReadOnlyCellData
        {
            public BoardCoordinate Coordinate { get; set; }
            public Cell? Cell;

            public ICharacter? Character { get; set; }

            public Vector3 WorldPosition => Cell!.GetWorldPosition();

            public override string ToString()
            {
                return $"Coordinate({Coordinate}) Cell({Cell}) Character({Character})";
            }
        }
    }
}