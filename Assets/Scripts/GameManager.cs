using System.Collections.Generic;
using State;
using System.Linq;
using UnityEngine;

#nullable disable

public sealed class GameManager
{
    private readonly Dictionary<StateType, IState> _stateMap;

    public GameManager()
    {
        var gameState = new GameState();
        _stateMap = new[] { gameState }.ToDictionary(x => x.GetStateType(), x => x as IState);

        Debug.Log($"{nameof(GameManager)} ctor {GetHashCode()}");
    }
}