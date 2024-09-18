using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

namespace Extensions
{
    public static class IListExtension
    {
        public static T? RandomPickClass<T>(this IList<T> list) where T : class
        {
            if (list.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, list.Count - 1);
            return list[randomIndex];
        }

        public static T? RandomPickStruct<T>(this IList<T> list) where T : struct
        {
            if (list.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, list.Count - 1);
            return list[randomIndex];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                return;
            }

            for (int i = 0 ; i < list.Count ; i++)
            {
                int randomIndex = Random.Range(0, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]); // swap
            }
        }
    }
}