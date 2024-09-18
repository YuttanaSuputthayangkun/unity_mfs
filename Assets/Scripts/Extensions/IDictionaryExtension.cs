using System.Collections.Generic;

namespace Extensions
{
    public static class IDictionaryExtension
    {
        public static TValue GetOrAddDefaultIfNotExist<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key)
            where TValue : new()
        {
            if (!map.ContainsKey(key))
            {
                map.Add(key, new());
            }

            return map[key];
        }
    }
}
