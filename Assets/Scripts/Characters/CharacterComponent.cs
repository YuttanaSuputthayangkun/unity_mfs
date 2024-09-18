using TMPro;
using UnityEngine;

#nullable enable

namespace Characters
{
    public class CharacterComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshPro numberText = null!;

        public void SetNumber(int? number)
        {
            numberText.text = number.ToString();
        }
    }
}