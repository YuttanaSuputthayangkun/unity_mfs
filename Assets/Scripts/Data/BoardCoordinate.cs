using System;

namespace Data
{
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
                Direction.Left => new BoardCoordinate(){ X = X - 1, Y = Y },
                Direction.Right => new BoardCoordinate(){ X = X + 1, Y = Y },
                Direction.Up => new BoardCoordinate(){ X = X, Y = Y - 1 },
                Direction.Down => new BoardCoordinate(){ X = X, Y = Y + 1 },
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public override string ToString() => $"({X}, {Y})";
        
        public static implicit operator BoardCoordinate((int x, int y) coordinate) => new(coordinate.x,coordinate.y);
    }
}
