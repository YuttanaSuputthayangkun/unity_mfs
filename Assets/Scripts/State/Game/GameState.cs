using System.Threading;
using Board;
using Cysharp.Threading.Tasks;
using Input;
using Unity.Mathematics;
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
            var setupBoardResult = _boardManager.SetupBoard();
            // Debug.Log($"setupBoardResult({setupBoardResult})");
            
            // set camera in the middle of the board
            _camera.transform.GetPositionAndRotation(out var originalPosition, out _);
            var newCameraPosition = originalPosition;
            newCameraPosition.x = setupBoardResult.BoardPosition.x;
            newCameraPosition.y = setupBoardResult.BoardPosition.y;
            _camera.transform.SetPositionAndRotation(newCameraPosition, Quaternion.identity);
            
            // set camera size to match with the board
            var newOrthographicSize = Mathf.Max(setupBoardResult.BoardSize.x, setupBoardResult.BoardSize.y) / 2;
            _camera.orthographicSize = newOrthographicSize;
            // Debug.Log($"newOrthographicSize({newOrthographicSize})");
        }
    }
}