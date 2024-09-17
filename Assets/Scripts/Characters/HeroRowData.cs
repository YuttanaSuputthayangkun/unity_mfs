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
        private readonly Hero _hero;

        public BoardCoordinate Coordinate => _coordinate;
        public HeroData HeroData => _heroData;

        public Hero Hero => _hero;

        public RowHeroData(BoardCoordinate coordinate, HeroData heroData, Hero hero)
        {
            this._coordinate = coordinate;
            this._heroData = heroData;
            _hero = hero;
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