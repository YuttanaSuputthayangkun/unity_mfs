using System.Threading;
using Board;
using Cysharp.Threading.Tasks;
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
        private readonly BoardManager _boardManager;
        private readonly Camera _camera;
        public StateType GetStateType() => StateType.GameState;

        public GameState(PlayerInputManager playerInputManager, BoardManager boardManager, Camera camera)
        {
            _playerInputManager = playerInputManager;
            _boardManager = boardManager;
            _camera = camera;
        }

        async UniTask IAsyncStartable.StartAsync(CancellationToken cancellation)
        {
            SetupScene(); 
            
            await UniTask.Yield();
            Debug.Log($"{nameof(GameState)} StartAsync");

            // one loop per turn
            while (true)
            {
                cancellation.ThrowIfCancellationRequested();
                
                var direction = await _playerInputManager.WaitDirectionalInputAsync(cancellation);
                Debug.Log($"{nameof(GameState)} StartAsync direction({direction})");
            }
        }

        private void SetupScene()
        {
            _boardManager.SetupBoard(); 
        }
    }
}