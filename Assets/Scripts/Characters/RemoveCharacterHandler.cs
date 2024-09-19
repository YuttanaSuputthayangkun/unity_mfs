using Board;
using Characters.Interfaces;

namespace Characters
{
    public class RemoveCharacterHandler
    {
        private readonly CharacterPool _characterPool;

        public RemoveCharacterHandler(CharacterPool characterPool)
        {
            _characterPool = characterPool;
        }
        
        public void RemoveCharacter(ICharacter character)
        {
            _characterPool.Push(character); 
        }
    }
}
