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
    public partial class BoardManager
    {
        public enum GetCellResultType
        {
            Found,
            OutOfBound,
        }

        public struct GetCellResult
        {
            public GetCellResultType ResultType;
            public bool IsFound => ResultType == GetCellResultType.Found;
            public IReadOnlyCellData? CellData;

            public override string ToString()
            {
                return ResultType.ToString();
            }
        }
    }
}
