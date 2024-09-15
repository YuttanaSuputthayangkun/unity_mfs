using Settings;

#nullable enable

namespace Characters
{
    public class CharacterSpawner
    {
        private readonly CharacterSpawnSetting _setting;

        public CharacterSpawner(CharacterSpawnSetting setting)
        {
            _setting = setting;
        }
    }
}