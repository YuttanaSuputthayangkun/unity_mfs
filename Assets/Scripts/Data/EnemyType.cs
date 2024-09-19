namespace Data
{
    public interface IContainEnemyType
    {
        EnemyType EnemyType { get; }
    }

    public enum EnemyType
    {
        Rogue,
        Wizard,
        Warrior,
    }
}