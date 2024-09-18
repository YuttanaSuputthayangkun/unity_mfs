using System;
using System.Collections.Generic;
using System.Linq;
using VContainer.Unity;

#nullable enable

namespace UI
{
    public class CharacterStatsListPanel : IStartable
    {
        private readonly CharacterStatsListPanelComponent _component;
        private readonly CharacterStatsPanel _characterStatsPanel;

        // TODO: move this into a setting
        private const int MaxPanelCount = 7;

        private CharacterStatsPanelComponent[]? _panelComponents = null;

        public CharacterStatsListPanel(
            CharacterStatsListPanelComponent component,
            CharacterStatsPanel characterStatsPanel
        )
        {
            _component = component;
            _characterStatsPanel = characterStatsPanel;
        }

        public bool TrySetHeroData(int index, CharacterPanelData data)
        {
            throw new NotImplementedException();
        }

        public void SetHeroDataList(IEnumerable<CharacterPanelData> dataList)
        {
            dataList.Take(MaxPanelCount);
        }

        private IEnumerable<CharacterStatsPanelComponent> GetPanels()
        {
            _panelComponents ??=
                _component.Content.GetComponentsInChildren<CharacterStatsPanelComponent>(includeInactive: true);

            return _panelComponents;
        }

        void IStartable.Start()
        {
            throw new System.NotImplementedException();
        }
    }
}