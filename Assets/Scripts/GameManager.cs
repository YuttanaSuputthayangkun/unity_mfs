using System.Collections.Generic;
using State;
using State.Game;
using System.Linq;

#nullable enable

public sealed class GameManager
{
    private readonly Dictionary<StateType, IState> _stateMap;

    public GameManager(GameState gameState)
    {
        _stateMap = new[] { gameState }.ToDictionary(x => x.GetStateType(), x => x as IState);
    }
}