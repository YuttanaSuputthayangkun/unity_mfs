using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Settings;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

#nullable enable

namespace Board
{
    public partial class BoardManager : MonoBehaviour
        , IInitializable
        , IDisposable
    {
        [Inject] private BoardSetting _boardSetting = null!;
        [Inject] private LifetimeScope _lifetimeScope = null!;

        private IDictionary<BoardCoordinate, CellData>? _cellDataMap = null;

        // to get random cells
        private List<BoardCoordinate>? _emptyCellCoordinateList = null;

        public SetupBoardResult SetupBoard()
        {
            Debug.Log($"{nameof(BoardManager)} SetupBoard");

            if (_boardSetting == null) throw new NullReferenceException(nameof(_boardSetting));

            var cellComponentPrefab = _boardSetting.CellComponentPrefab;

            // create cells and populate the map
            using (LifetimeScope.EnqueueParent(_lifetimeScope))
            {
                var newCellDataMap = new Dictionary<BoardCoordinate, CellData>();
                foreach (int x in Enumerable.Range(0, _boardSetting.BoardWidth))
                {
                    foreach (int y in Enumerable.Range(0, _boardSetting.BoardHeight))
                    {
                        var coordinate = new BoardCoordinate(x, y);
                        var newCell = _lifetimeScope.Container.Resolve<Cell>();
                        newCell.SetName(coordinate.ToString());
                        var newCellData = new CellData
                        {
                            Coordinate = coordinate,
                            Cell = newCell,
                        };
                        newCellDataMap.Add(coordinate, newCellData);
                    }
                }

                _cellDataMap = newCellDataMap;
            }

            // every cells are empty cells
            _emptyCellCoordinateList = _cellDataMap.Select(x => x.Key).ToList();

            // set positions of the cells
            {
                Vector3 basePosition = new Vector3(0, 0, 0);
                Vector2 cellSize = cellComponentPrefab.SpriteRenderer.size;
                // Debug.Log($"{nameof(cellSize)}({cellSize})");
                foreach (int x in Enumerable.Range(0, _boardSetting.BoardWidth))
                {
                    foreach (int y in Enumerable.Range(0, _boardSetting.BoardHeight))
                    {
                        var coordinate = new BoardCoordinate(x, y);
                        var cell = _cellDataMap[coordinate].Cell;

                        var newPosition = new Vector3(
                            basePosition.x + cellSize.x * x,
                            basePosition.y - cellSize.y * y,
                            0
                        );
                        cell!.SetPosition(newPosition);
                        // Debug.Log($"cell({cell.name}) position({newPosition})", cell);
                    }
                }
            }

            // produce setup result
            var topLeftCell = _cellDataMap[(0, 0)].Cell;
            var bottomRightCell =
                _cellDataMap[(_boardSetting.BoardHeight - 1, _boardSetting.BoardWidth - 1)].Cell;

            var result = new SetupBoardResult
            {
                BoardPosition = (bottomRightCell!.Position - topLeftCell!.Position) / 2,
                BoardSize = new Vector2(
                    cellComponentPrefab.SpriteRenderer.size.x * _boardSetting.BoardWidth,
                    cellComponentPrefab.SpriteRenderer.size.y * _boardSetting.BoardHeight
                )
            };
            return result;
        }

        void IInitializable.Initialize()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            // TODO: consider cleanup the cells if necessary
            _cellDataMap = null;
        }

        public GetCellResult GetCell(BoardCoordinate coordinate)
        {
            if (_cellDataMap is null)
            {
                throw new NullReferenceException($"Please call {nameof(SetupBoard)}.");
            }

            if (!_cellDataMap.TryGetValue(coordinate, out var cellData))
            {
                return new GetCellResult() { ResultType = GetCellResultType.OutOfBound };
            }

            return new GetCellResult() { ResultType = GetCellResultType.Found, CellData = cellData };
        }

        public GetCellResult GetRandomEmptyCell()
        {
            if (_cellDataMap is null || _emptyCellCoordinateList is null)
            {
                throw new NullReferenceException($"Please call {nameof(SetupBoard)}.");
            }

            int randomEmptyCellIndex = Random.Range(0, _emptyCellCoordinateList.Count - 1);
            var randomCoordinate = _emptyCellCoordinateList[randomEmptyCellIndex];
            return GetCell(randomCoordinate);
        }

        public SetCellResult SetCellObjectType(BoardCoordinate boardCoordinate, BoardObjectType boardObjectType)
        {
            if (_cellDataMap is null || _emptyCellCoordinateList is null)
            {
                throw new NullReferenceException($"Please call {nameof(SetupBoard)}.");
            }

            var getResult = GetCell(boardCoordinate);
            if (getResult is { ResultType: GetCellResultType.OutOfBound })
            {
                return new SetCellResult() { ResultType = SetCellResultType.OutOfBound };
            }

            _cellDataMap[boardCoordinate].BoardObjectType = boardObjectType;
            
            return new SetCellResult() { ResultType = SetCellResultType.Set };
        }
    }
}