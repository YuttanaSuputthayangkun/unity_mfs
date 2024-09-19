using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine.InputSystem.XInput;

#nullable enable

namespace Characters
{
    public partial class HeroRow
    {
        // an internal class to keep the list of heroes
        private class HeroList
        {
            public enum RotateType
            {
                Forward,
                Back,
            }

            private readonly Queue<Hero> _heroQueue = new();

            // use this to check which hero is in the row
            private readonly HashSet<Hero> _heroSet = new();

            private Hero? _first = null;

            private Hero? _last = null;

            public IEnumerable<Hero> RowHeroDataList => _heroQueue;

            public int Count => _heroQueue.Count;

            public Hero? GetFirst() => _first;

            public Hero? GetLast() => _last;

            public override string ToString()
            {
                var characterStrings = _heroQueue.Select((x, i) => $"[{i}]{x}");
                return string.Join("\n", characterStrings);
            }

            public bool ContainsHero(Hero hero)
            {
                return _heroSet.Contains(hero);
            }

            public int? GetHeroIndex(Hero hero)
            {
                // TODO: do some index caching
                return _heroQueue
                    .Select((x, i) => (x, i) as (Hero hero, int index)?)
                    .FirstOrDefault(x => x!.Value.hero == hero)
                    ?.index;
            }

            public void Add(Hero hero)
            {
                _heroQueue.Enqueue(hero);
                _heroSet.Add(hero);
                _first ??= hero;
                _last = hero;
            }

            public Hero RemoveFirst()
            {
                var first = _heroQueue.Dequeue()!;
                _heroSet.Remove(first);
                _first = _heroQueue.TryPeek(out var newFirst) ? newFirst : null; // move new first to first
                return first;
            }

            public void Rotate(RotateType rotateType)
            {
                throw new NotImplementedException();
            }
        }
    }
}