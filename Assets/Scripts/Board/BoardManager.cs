using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Settings;
using UnityEngine;
using VContainer;

#nullable enable

namespace Board
{
    public partial class BoardManager : MonoBehaviour
        , IDisposable
    {
        [SerializeField] private Cell _cellPrefab = null!;
        [SerializeField] private Transform boardPositionReference = null!;

        [Inject] private readonly BoardSetting _boardSetting = null!;

        // consider changing into IReadonlyDictionary
        Dictionary<BoardCoordinate, CellData>? cellDataMap = null;

        public void SetupBoard()
        {
            Debug.Log($"{nameof(BoardManager)} SetupBoard");
            // TODO: implement 

            cellDataMap = new Dictionary<BoardCoordinate, CellData>();
            foreach (uint x in Enumerable.Range(0, (int)_boardSetting.BoardWidth))
            {
                foreach (uint y in Enumerable.Range(0, (int)_boardSetting.BoardHeight))
                {
                }
            }
        }

        void IDisposable.Dispose()
        {
            // TODO: consider cleanup the cells if necessary
            cellDataMap = null;
        }

        public GetCellResult GetCell(BoardCoordinate coordinate)
        {
            if (cellDataMap is null)
            {
                throw new NullReferenceException($"Please call {nameof(SetupBoard)}.");
            }

            if (!cellDataMap.TryGetValue(coordinate, out var cellData))
            {
                return new GetCellResult() { ResultType = GetCellResultType.OutOfBound };
            }

            return new GetCellResult() { ResultType = GetCellResultType.Found, CellData = cellData };
        }
    }
}