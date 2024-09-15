using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Input;
using UnityEngine;
using VContainer.Unity;

#nullable enable

namespace State.Game
{
    public class GameState : IState
        , IAsyncStartable
    {
        private readonly PlayerInputManager _playerInputManager;
        public StateType GetStateType() => StateType.GameState;

        public GameState(PlayerInputManager playerInputManager)
        {
            _playerInputManager = playerInputManager;
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation)
        {
            await UniTask.Yield();
            Debug.Log($"{nameof(GameState)} StartAsync");

            while (true)
            {
                var direction = await _playerInputManager.WaitDirectionalInputAsync(cancellation);
                Debug.Log($"{nameof(GameState)} StartAsync direction({direction})");
            }

            // Debug.Log($"{nameof(GameState)} StartAsync end direction({direction})");
        }
    }
}