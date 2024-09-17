namespace Data
{
    [System.Serializable]
    public struct CharacterStats
    {
        public int health;
        public int attack;
        public int defense;

        public override string ToString()
        {
            return $"H({health}) A({attack}) D({defense})";
        }
    }
}