#nullable enable

using System.Collections.Generic;

namespace Extensions
{
    public static class IEnumerableExtension
    {
        // get a list of an element paired with the previous element
        public static IEnumerable<(T?, T)> ToPairsWithPreviousStruct<T>(this IEnumerable<T> list) where T : struct
        {
            T? previous = null;
            foreach (var element in list)
            {
                yield return (previous, element);
                previous = element;
            }
        }

        // get a list of an element paired with the previous element
        public static IEnumerable<(T?, T)> ToPairsWithPreviousClass<T>(this IEnumerable<T> list) where T : class
        {
            T? previous = null;
            foreach (var element in list)
            {
                yield return (previous, element);
                previous = element;
            }
        }
    }
}