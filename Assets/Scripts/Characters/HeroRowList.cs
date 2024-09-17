using System;
using System.Collections.Generic;
using System.Linq;
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

            private readonly Queue<RowHeroData> _heroQueue = new();

            // use this to check which hero is in the row
            private readonly HashSet<Hero> _heroSet = new();

            private RowHeroData? _first = null;

            private RowHeroData? _last = null;

            public IEnumerable<RowHeroData> RowHeroDataList => _heroQueue;

            public int Count => _heroQueue.Count;

            public RowHeroData? GetFirst() => _first;

            public RowHeroData? GetLast() => _last;

            public override string ToString()
            {
                var characterStrings = _heroQueue.Select((x, i) => $"[{i}]{x}");
                return string.Join("\n", characterStrings);
            }

            public bool ContainsHero(Hero hero)
            {
                return _heroSet.Contains(hero);
            }

            public void Add(RowHeroData heroData)
            {
                _heroQueue.Enqueue(heroData);
                _heroSet.Add(heroData.Hero);
                _first ??= heroData;
                _last = heroData;
            }

            public void Rotate(RotateType rotateType)
            {
                throw new NotImplementedException();
            }
        }
    }
}