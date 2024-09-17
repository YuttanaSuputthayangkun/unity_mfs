namespace Data
{
    public interface IContainBoardObjectType
    {
        BoardObjectType BoardObjectType { get; }
    }
    
    public enum BoardObjectType 
    {
        Hero,
        Enemy,
        Obstacle
    }
}
