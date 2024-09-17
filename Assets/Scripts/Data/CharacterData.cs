using Characters;
using UnityEngine;

namespace Data
{
    public interface IReadOnlyCharacterData<TCharacterType>
    {
        TCharacterType Type { get; }
        CharacterStats Stats { get; }
    }

    [System.Serializable]
    public class CharacterData<TCharacterType> : IReadOnlyCharacterData<TCharacterType>
    {
        [SerializeField] private TCharacterType characterType;
        [SerializeField] private CharacterStats stats;

        public TCharacterType Type => characterType;

        public CharacterStats Stats => stats;

        public override string ToString()
        {
            return $"Type({characterType}) Stat({stats})";
        }
    }

    [System.Serializable]
    public class HeroData : CharacterData<HeroType>
    {
    }

    [System.Serializable]
    public class EnemyData : CharacterData<EnemyType>
    {
    }

    [System.Serializable]
    public class ObstacleData : CharacterData<ObstacleType>
    {
    }
}