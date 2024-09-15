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
        [SerializeField] private Transform cellParent = null!;
        [SerializeField] private Transform boardPositionReference = null!;

        [Inject] private BoardSetting _boardSetting = null!;

        IReadOnlyDictionary<BoardCoordinate, CellData>? cellDataMap = null;

        public SetupBoardResult SetupBoard()
        {
            Debug.Log($"{nameof(BoardManager)} SetupBoard");

            if (_boardSetting == null) throw new NullReferenceException(nameof(_boardSetting));

            // create cells and populate the map
            var newCellDataMap = new Dictionary<BoardCoordinate, CellData>();
            foreach (int x in Enumerable.Range(0, _boardSetting.BoardWidth))
            {
                foreach (int y in Enumerable.Range(0, _boardSetting.BoardHeight))
                {
                    var coordinate = new BoardCoordinate(x, y);
                    var newCell = Instantiate(_cellPrefab, parent: cellParent);
                    newCell.name = coordinate.ToString();
                    var newCellData = new CellData
                    {
                        Coordinate = coordinate,
                        Cell = newCell,
                    };
                    newCellDataMap.Add(coordinate, newCellData);
                }
            }

            cellDataMap = newCellDataMap;

            // set positions of the cells
            {
                Vector3 basePosition = new Vector3(0, 0, 0);
                Vector2 cellSize = _cellPrefab.SpriteRenderer.size;
                // Debug.Log($"{nameof(cellSize)}({cellSize})");
                foreach (int x in Enumerable.Range(0, _boardSetting.BoardWidth))
                {
                    foreach (int y in Enumerable.Range(0, _boardSetting.BoardHeight))
                    {
                        var coordinate = new BoardCoordinate(x, y);
                        var cell = cellDataMap[coordinate].Cell;

                        var newPosition = new Vector3(
                            basePosition.x + cellSize.x * x,
                            basePosition.y - cellSize.y * y,
                            0
                        );
                        cell.transform
                            .SetLocalPositionAndRotation(newPosition, Quaternion.identity);
                        // Debug.Log($"cell({cell.name}) position({newPosition})", cell);
                    }
                }
            }

            var topLeftCell = cellDataMap[(0, 0)].Cell;
            var bottomRightCell =
                cellDataMap[(_boardSetting.BoardHeight - 1, _boardSetting.BoardWidth - 1)].Cell;

            var result = new SetupBoardResult
            {
                BoardPosition = (bottomRightCell.transform.position - topLeftCell.transform.position) / 2,
                BoardSize = new Vector2(
                    _cellPrefab.SpriteRenderer.size.x * _boardSetting.BoardWidth,
                    _cellPrefab.SpriteRenderer.size.y * _boardSetting.BoardHeight
                )
            };
            return result;
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