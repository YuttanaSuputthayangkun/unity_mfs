using System.Collections.Generic;
using Characters.Interfaces;
using Data;

#nullable enable

namespace Characters
{
    public class CharacterPool
    {
        private readonly CharacterPoolComponent _characterPoolComponent;

        private readonly Dictionary<CharacterType, Queue<ICharacter>> queueMap = new();

        public CharacterPool(CharacterPoolComponent characterPoolComponent)
        {
            _characterPoolComponent = characterPoolComponent;
        }

        public void Push(ICharacter character)
        {
            character.SetWorldPosition(_characterPoolComponent.transform.position);

            var characterType= character.GetCharacterType();
            if (!queueMap.ContainsKey(characterType))
            {
                queueMap.Add(characterType, new()); 
            }
            queueMap[characterType].Enqueue(character);
        }

        public ICharacter? Pop(CharacterType characterType)
        {
            if (!queueMap.TryGetValue(characterType, out var queue))
            {
                return null;
            }
            return queue.TryDequeue(out var dequeue) ? dequeue : null;
        }
    }
}