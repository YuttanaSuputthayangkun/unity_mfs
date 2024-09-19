using Data;
using TMPro;
using UnityEngine;

#nullable enable

namespace Characters
{
    public class CharacterComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshPro numberText = null!;
        [SerializeField] private TextMeshPro statsText = null!;

        public void SetNumber(int? number)
        {
            numberText.text = number.ToString();
        }

        public void SetStats(ICharacterStats characterStats)
        {
            var newText = $"<color=red>{characterStats.Attack}</color>" +
                          $"<color=blue>{characterStats.Defense}</color>" +
                          $"<color=green>{characterStats.Health}</color>";
            statsText.text = newText;
        }
    }
}