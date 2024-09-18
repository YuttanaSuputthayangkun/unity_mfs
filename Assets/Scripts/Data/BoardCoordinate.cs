using System;
using System.Xml;
using UnityEngine.InputSystem.XInput;

namespace Data
{
    [System.Serializable]
    public struct BoardCoordinate 
    {
        public int X;
        public int Y;

        public BoardCoordinate(int x, int y) : this()
        {
            this.X = x;
            this.Y = y;
        }

        public BoardCoordinate GetNeighbor(Direction direction)
        {
            return direction switch
            {
                Direction.Left => new BoardCoordinate() { X = X - 1, Y = Y },
                Direction.Right => new BoardCoordinate() { X = X + 1, Y = Y },
                Direction.Up => new BoardCoordinate() { X = X, Y = Y - 1 },
                Direction.Down => new BoardCoordinate() { X = X, Y = Y + 1 },
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public Direction? TryGetNeighborDirection(BoardCoordinate otherCoordinate)
        {
            return (otherCoordinate.X, otherCoordinate.Y) switch
            {
                var (ox, oy) when ox == X - 1 && oy == Y => Direction.Left,
                var (ox, oy) when ox == X + 1 && oy == Y => Direction.Right,
                var (ox, oy) when ox == X && oy == Y - 1 => Direction.Up,
                var (ox, oy) when ox == X && oy == Y + 1 => Direction.Down,
                _ => null,
            };
        }

        public bool IsNeighbor(BoardCoordinate otherCoordinate)
        {
            foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                if (GetNeighbor(direction) == otherCoordinate)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString() => $"({X}, {Y})";

        public static implicit operator BoardCoordinate((int x, int y) coordinate) => new(coordinate.x, coordinate.y);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                BoardCoordinate other => this == other,
                _ => false,
            };
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(BoardCoordinate a, BoardCoordinate b)
        {
            return a.X == b.X
                   && a.Y == b.Y;
        }

        public static bool operator !=(BoardCoordinate a, BoardCoordinate b)
        {
            return !(a == b);
        }
    }
}