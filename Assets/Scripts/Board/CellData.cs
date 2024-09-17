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
            BoardObjectType? BoardObjectType { get; set; }
            bool IsOccupied { get; }
            Vector3 WorldPosition { get; }
        }

        private class CellData : IReadOnlyCellData
        {
            public BoardCoordinate Coordinate { get; set; }
            public Cell? Cell;

            public BoardObjectType? BoardObjectType { get; set; }

            public bool IsOccupied => BoardObjectType is not null;

            public Vector3 WorldPosition => Cell!.GetWorldPosition();
        }
    }
}