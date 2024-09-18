using System.Collections.Generic;
using System.Linq;
using Characters.Interfaces;

#nullable enable

namespace Characters
{
    public class NonPlayerCharacterList
    {
        private readonly HashSet<ICharacter> _characterList = new();

        public void Push(ICharacter character)
        {
            _characterList.Add(character);
        }

        public ICharacter Remove()
        {
            var first = _characterList.First();
            _characterList.Remove(first);
            return first;
        }

        public void Remove(ICharacter character)
        {
            _characterList.Remove(character);
        }

        public bool Contains(ICharacter character)
        {
            return _characterList.Contains(character);
        }
    }
}