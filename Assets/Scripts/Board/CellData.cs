using System;
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
        }

        private struct CellData : IReadOnlyCellData
        {
            public BoardCoordinate Coordinate { get; set; }
            public Cell Cell;

            // TODO: add some information here
            // Is there anything occupied here?

            public BoardObjectType? BoardObjectType { get; set; }

            public bool IsOccupied => BoardObjectType is not null;
        }
    }
}