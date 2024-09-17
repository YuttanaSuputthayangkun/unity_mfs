using Characters;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class CharacterData<TCharacterType>
    {
        [SerializeField]
        public TCharacterType characterType;
        [SerializeField]
        public CharacterStats stats;

        public TCharacterType Type => characterType;

        public CharacterStats Stats => stats;
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