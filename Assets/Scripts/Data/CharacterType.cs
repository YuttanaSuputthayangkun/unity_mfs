namespace Data
{
    public interface IContainCharacterType
    {
        CharacterType CharacterType { get; }
    }
    
    public enum CharacterType
    {
        Hero,
        Enemy,
        Obstacle,
    }
}