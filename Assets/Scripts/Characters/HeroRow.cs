using System;
using System.Collections.Generic;
using Board;
using System.Linq;

#nullable enable

namespace Characters
{
    public interface IContainReadOnlyRowHeroDataList
    {
        IEnumerable<IReadOnlyRowHeroData> RowHeroDataList { get; }
    }

    public class HeroReadOnlyRow :
        IContainReadOnlyRowHeroDataList
    {
        private readonly BoardManager _boardManager;

        private readonly Queue<RowHeroData> _characterQueue = new Queue<RowHeroData>();

        public HeroReadOnlyRow(BoardManager boardManager)
        {
            _boardManager = boardManager;
        }

        public IEnumerable<IReadOnlyRowHeroData> RowHeroDataList => _characterQueue;

        public override string ToString()
        {
            var characterStrings = _characterQueue.Select((x, i) => $"[{i}]{x}");
            return string.Join("\n", characterStrings);
        }
    }
}