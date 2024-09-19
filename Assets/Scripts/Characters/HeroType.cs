namespace Characters
{
    public interface IContainHeroType
    {
        HeroType HeroType { get; }
    } 
    
    public enum HeroType
    {
        Rogue,
        Wizard,
        Warrior,
    }
}