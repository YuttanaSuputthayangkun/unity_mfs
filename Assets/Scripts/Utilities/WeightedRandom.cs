using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    public class WeightedRandom<T>
    {
        private readonly List<float> _weightList = new();
        private readonly List<T> _itemList = new();

        public WeightedRandom(IEnumerable<(T, int)> list)
        {
            float currentMaxWeight = 0;
            foreach (var (item, weight) in list)
            {
                currentMaxWeight += weight;
                _weightList.Add(currentMaxWeight);
                
                _itemList.Add(item);
            }
        }

        public T GetRandomItem()
        {
            var randomValue = Random.Range(0, _weightList.Last());
            int index = _weightList.BinarySearch(randomValue);
            if (index < 0) index = ~index;
            return _itemList[index];
        }
    }
}