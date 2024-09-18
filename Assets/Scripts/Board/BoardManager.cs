using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Characters.Interfaces;
using Data;
using Extensions;
using Settings;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

namespace Board
{
    public partial class BoardManager : MonoBehaviour
    {
        [Inject] private BoardSetting _boardSetting = null!;
        [Inject] private LifetimeScope _lifetimeScope = null!;

        private Dictionary<BoardCoordinate, CellData>? _cellDataMapByCoordinate = null;
        private Dictionary<ICharacter, BoardCoordinate>? _coordinateMapByCharacter = null;

        // to get random cells
        private HashSet<BoardCoordinate>? _emptyCellCoordinateList = null;

        public string GetDebugText()
        {
            StringBuilder sb = new();
            var joinedCellDataMapByCoordinate =
                string.Join("\n", _cellDataMapByCoordinate!.Select((x => $"{x.Key} {x.Value}")));
            sb.Append($"joinedCellDataMapByCoordinate:\n{joinedCellDataMapByCoordinate}\n");
            var joinedCoordinateMapByCharacter =
                string.Join("\n", _coordinateMapByCharacter!.Select((x => $"{x.Key} {x.Value}")));
            sb.Append($"joinedCoordinateMapByCharacter:\n{joinedCoordinateMapByCharacter}\n");
            var joinedEmptyCellCoordinateList =
                string.Join("\n", _emptyCellCoordinateList!.Select((x => x.ToString())));
            sb.Append($"joinedEmptyCellCoordinateList:\n{joinedEmptyCellCoordinateList}\n");
            return sb.ToString();
        }

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

                _cellDataMapByCoordinate = newCellDataMap;
            }

            // every cells are empty cells
            var newEmptyCellCoordinateList = _cellDataMapByCoordinate.Select(x => x.Key).ToArray();
            newEmptyCellCoordinateList.Shuffle(); // shuffle now so we don't have to do it again when random spawn 
            _emptyCellCoordinateList = newEmptyCellCoordinateList.ToHashSet();

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
                        var cell = _cellDataMapByCoordinate[coordinate].Cell;

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
            var topLeftCell = _cellDataMapByCoordinate[(0, 0)].Cell;
            var bottomRightCell =
                _cellDataMapByCoordinate[(_boardSetting.BoardHeight - 1, _boardSetting.BoardWidth - 1)].Cell;

            _coordinateMapByCharacter = new();

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

        public GetCellResult GetCell(BoardCoordinate coordinate)
        {
            ThrowIfNotSetup();

            if (!_cellDataMapByCoordinate!.TryGetValue(coordinate, out var cellData))
            {
                return new GetCellResult() { ResultType = GetCellResultType.OutOfBound };
            }

            return new GetCellResult() { ResultType = GetCellResultType.Found, CellData = cellData };
        }

        public GetCellResult GetCell(ICharacter character)
        {
            ThrowIfNotSetup();

            if (!_coordinateMapByCharacter!.TryGetValue(character, out var matchedCoordinate))
            {
                Debug.Log($"GetCell character not found:\n{GetDebugText()}");
                return new GetCellResult() { ResultType = GetCellResultType.NoCharacterOnBoard };
            }

            return GetCell(matchedCoordinate);
        }

        public int GetEmptyCellCount()
        {
            ThrowIfNotSetup();

            return _emptyCellCoordinateList!.Count;
        }

        public GetCellResult GetRandomEmptyCell() => GetRandomEmptyCells(1).First();

        public IEnumerable<GetCellResult> GetRandomEmptyCells(int count)
        {
            ThrowIfNotSetup();

            return _emptyCellCoordinateList!.Take(count).Select(GetCell);
        }

        public PlaceCharacterResult PlaceCharacter(BoardCoordinate boardCoordinate, ICharacter character)
        {
            ThrowIfNotSetup();

            var getResult = GetCell(boardCoordinate);
            if (getResult is { ResultType: GetCellResultType.OutOfBound })
            {
                return new PlaceCharacterResult()
                    { ResultType = PlaceCharacterResultType.OutOfBound };
            }

            if (getResult.CellData?.Character is { })
            {
                return new PlaceCharacterResult() { ResultType = PlaceCharacterResultType.CellOccupied };
            }

            SetCharacterCell(boardCoordinate, character);

            return new PlaceCharacterResult() { ResultType = PlaceCharacterResultType.Placed };
        }

        public RemoveCharacterFromCellResult RemoveCharacter(BoardCoordinate boardCoordinate)
        {
            ThrowIfNotSetup();

            var getResult = GetCell(boardCoordinate);
            if (getResult is { ResultType: GetCellResultType.OutOfBound })
            {
                return new RemoveCharacterFromCellResult()
                    { ResultType = RemoveCharacterFromCellResultType.OutOfBound };
            }

            var existingCellData = _cellDataMapByCoordinate![boardCoordinate];

            if (existingCellData.Character is { } existingCharacter)
            {
                // remove existing character on cell by reverse map, for it to be moved to the new cell

                existingCellData.Character = null;

                _coordinateMapByCharacter!.Remove(existingCharacter);

                // to remove from the board(physically)
                // existingCharacter.Remove();
                // TODO: make sure to call this somewhere else
            }

            // by removing character, add this coordinate to empty list
            _emptyCellCoordinateList!.Add(boardCoordinate);

            return new RemoveCharacterFromCellResult()
                { ResultType = RemoveCharacterFromCellResultType.Removed };
        }

        public MoveCharacterResult MoveCharacter(BoardCoordinate boardCoordinate, ICharacter character)
        {
            ThrowIfNotSetup();

            var getResult = GetCell(boardCoordinate);
            if (getResult is { ResultType: GetCellResultType.OutOfBound })
            {
                return new MoveCharacterResult() { ResultType = MoveCharacterResultType.OutOfBound };
            }

            if (getResult.CellData?.Character is { })
            {
                return new MoveCharacterResult() { ResultType = MoveCharacterResultType.CellOccupied };
            }

            var currentCharacterCoordinate = character.GetBoardCoordinate();
            if (currentCharacterCoordinate is null)
            {
                return new MoveCharacterResult() { ResultType = MoveCharacterResultType.CharacterIsNotOnBoard };
            }

            if (currentCharacterCoordinate == boardCoordinate)
            {
                return new MoveCharacterResult() { ResultType = MoveCharacterResultType.SameCoordinate };
            }


            // we don't have to remove by this method any more, because we just removed character.remove call from
            // RemoveCharacter method, so now we can use it here
            RemoveCharacter(currentCharacterCoordinate!.Value);
            // no need to care the result, we just need to make sure it's removed

            SetCharacterCell(boardCoordinate, character);

            return new MoveCharacterResult() { ResultType = MoveCharacterResultType.Moved };
        }

        private void SetCharacterCell(BoardCoordinate boardCoordinate, ICharacter character)
        {
            var cellData = _cellDataMapByCoordinate![boardCoordinate];

            cellData.Character = character;
            _coordinateMapByCharacter![character] = boardCoordinate;
            // Debug.Log($"set coordinate map by character({character}) coordinate({boardCoordinate})");

            // by adding character, remove this coordinate to empty list
            _emptyCellCoordinateList!.Remove(boardCoordinate);

            // physically move the character
            character?.SetWorldPosition(cellData.Cell!.GetWorldPosition());

            // Debug.Log($"SetCellCharacter character null? ({character is null})\n{GetDebugText()}");
        }

        private void ThrowIfNotSetup()
        {
            if (
                _cellDataMapByCoordinate is null
                || _emptyCellCoordinateList is null
                || _coordinateMapByCharacter is null
            )
            {
                throw new NullReferenceException($"Please call {nameof(SetupBoard)}.");
            }
        }
    }
}