using Board;
using Settings;

#nullable enable

namespace Characters
{
    public class CharacterSpawner
    {
        public struct CharacterSpawningResult
        {
            
        } 
        
        private readonly BoardManager _boardManager;
        private readonly CharacterSpawnSetting _setting;

        public CharacterSpawner(BoardManager boardManager, CharacterSpawnSetting setting)
        {
            _boardManager = boardManager;
            _setting = setting;
        }
        
        public CharacterSpawningResult SpawnCharacter()
        {
             
            
            // TODO: implement this
            return new CharacterSpawningResult();
        }
        
        public CharacterSpawningResult SpawnRandomCharacter()
        {
            // TODO: implement this
            return new CharacterSpawningResult();
        }
    }
}