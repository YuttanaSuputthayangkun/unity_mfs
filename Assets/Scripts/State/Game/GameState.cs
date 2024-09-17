using System;
using System.Threading;
using Board;
using Characters;
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
        , IDisposable
    {
        private struct PlayerInputResult
        {
            public bool Walked;
            public bool RotatedHero;
        }

        private struct CollisionProcessResult
        {
            public bool HasCollision;
        }

        private readonly PlayerInputManager _playerInputManager;
        private readonly BoardManager _boardManager;
        private readonly Camera _camera;
        private readonly CharacterSpawner _characterSpawner;
        public StateType GetStateType() => StateType.GameState;

        public GameState(PlayerInputManager playerInputManager, BoardManager boardManager, Camera camera,
            CharacterSpawner characterSpawner)
        {
            _playerInputManager = playerInputManager;
            _boardManager = boardManager;
            _camera = camera;
            _characterSpawner = characterSpawner;
        }

        UniTask IAsyncStartable.StartAsync(CancellationToken cancellation) => PlayAsync(cancellation);

        public async UniTask PlayAsync(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            SetupScene();
            SetupStartHero();
            SpawnCharacters();

            await UniTask.Yield();
            Debug.Log($"{nameof(GameState)} StartAsync");

            // one loop per turn
            while (true)
            {
                cancellation.ThrowIfCancellationRequested();

                var direction = await _playerInputManager.WaitDirectionalInputAsync(cancellation);
                Debug.Log($"{nameof(GameState)} StartAsync direction({direction})");

                var collisionProcessResult = ProcessCollision();
                if (collisionProcessResult.HasCollision)
                {
                    SpawnCharacters();
                }
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

        private void SetupStartHero()
        {
            
        }

        private CollisionProcessResult ProcessCollision()
        {
            // TODO: implement this

            return new CollisionProcessResult();
        }

        private void SpawnCharacters()
        {
        }

        void IDisposable.Dispose()
        {
            // consider cleaning up the board
        }
    }
}