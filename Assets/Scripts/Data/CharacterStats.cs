namespace Data
{
    public interface ICharacterStats
    {
        int Health { get; }
        int Attack { get; }
        int Defense { get; }
    }

    public interface ISetCharacterStats
    {
        void SetCharacterStats(ICharacterStats characterStats);
    }

    [System.Serializable]
    public struct CharacterStats : ICharacterStats
    {
        public int health;
        public int attack;
        public int defense;

        public override string ToString()
        {
            return $"H({health}) A({attack}) D({defense})";
        }

        public int Health => health;
        public int Attack => attack;
        public int Defense => defense;
    }
}