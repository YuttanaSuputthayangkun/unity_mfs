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

        public CharacterData()
        {
        }

        public CharacterData(TCharacterType characterType, CharacterStats stats) : this()
        {
            this.characterType = characterType;
            this.stats = stats;
        }

        public CharacterData(IReadOnlyCharacterData<TCharacterType> readOnlyData) : this(
            readOnlyData.Type,
            readOnlyData.Stats
        )
        {
        }

        public override string ToString()
        {
            return $"Type({characterType}) Stat({stats})";
        }
    }

    [System.Serializable]
    public class HeroData : CharacterData<HeroType>
    {
        public HeroData(IReadOnlyCharacterData<HeroType> readOnlyData) : base(readOnlyData)
        {
        }
    }

    [System.Serializable]
    public class EnemyData : CharacterData<EnemyType>
    {
        public EnemyData(IReadOnlyCharacterData<EnemyType> readOnlyData) : base(readOnlyData)
        {
        }
    }

    [System.Serializable]
    public class ObstacleData : CharacterData<ObstacleType>
    {
        public ObstacleData(IReadOnlyCharacterData<ObstacleType> readOnlyData) : base(readOnlyData)
        {
        }
    }
}