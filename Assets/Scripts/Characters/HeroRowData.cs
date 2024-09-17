using Data;

namespace Characters
{
    public interface IReadOnlyRowHeroData
    {
        BoardCoordinate Coordinate { get; }
        HeroData HeroData { get; }
    }
    
    public class RowHeroData : IReadOnlyRowHeroData
    {
        private BoardCoordinate _coordinate;
        private readonly HeroData _heroData;

        public BoardCoordinate Coordinate => _coordinate;
        public HeroData HeroData => _heroData;

        public RowHeroData(BoardCoordinate coordinate, HeroData heroData)
        {
            this._coordinate = coordinate;
            this._heroData = heroData;
        }

        public void UpdateCoordinate(BoardCoordinate coordinate)
        {
            _coordinate = coordinate;
        }

        public override string ToString()
        {
            return $"{_coordinate} {_heroData}";
        }
    }
}