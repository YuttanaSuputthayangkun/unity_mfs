using System.Collections.Generic;
using State;
using System.Linq;
using Input;
using UnityEngine;

#nullable enable

public sealed class GameManager
{
    private readonly PlayerInputManager _playerInputManager;
    private readonly Dictionary<StateType, IState> _stateMap;
    
    // injected
    // private PlayerInput playerInput;

    public GameManager(/*PlayerInput playerInput*/ PlayerInputManager playerInputManager)
    {
        _playerInputManager = playerInputManager;
        var gameState = new GameState();
        _stateMap = new[] { gameState }.ToDictionary(x => x.GetStateType(), x => x as IState);
        // Debug.Log($"{nameof(GameManager)} ctor {GetHashCode()}");

        // this.playerInput = playerInput;
        // Debug.Log($"{nameof(GameManager)} ctor {nameof(this.playerInput)}({this.playerInput.GetHashCode()})");
        
        Debug.Log($"{nameof(GameManager)} ctor {nameof(this._playerInputManager)}({this._playerInputManager.GetHashCode()})");
    }
}