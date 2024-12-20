using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace Extensions
{
    public static class IReadOnlyListExtension 
    {
        public static T? RandomPickClass<T>(this IReadOnlyList<T> list) where T : class
        {
            if (list.Count == 0)
            {
                return null;
            }
            
            int randomIndex = Random.Range(0, list.Count - 1);
            return list[randomIndex];
        }
        
        public static T? RandomPickStruct<T>(this IReadOnlyList<T> list) where T : struct
        {
            if (list.Count == 0)
            {
                return null;
            }
            
            int randomIndex = Random.Range(0, list.Count - 1);
            return list[randomIndex];
        }
    }
}
