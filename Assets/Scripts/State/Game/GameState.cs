using System;
using System.Threading;
using Board;
using Characters;
using Cysharp.Threading.Tasks;
using Data;
using Input;
using Settings;
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

        private struct CollisionCheckResult
        {
            public object? CollidedObject;
            public bool HasCollision => CollidedObject is not null;
        }

        private readonly PlayerInputManager _playerInputManager;
        private readonly BoardManager _boardManager;
        private readonly Camera _camera;
        private readonly CharacterSpawner _characterSpawner;
        private readonly HeroRow _heroRow;
        private readonly BoardSetting _boardSetting;
        public StateType GetStateType() => StateType.GameState;

        public GameState(PlayerInputManager playerInputManager, BoardManager boardManager, Camera camera,
            CharacterSpawner characterSpawner, HeroRow heroRow, BoardSetting boardSetting)
        {
            _playerInputManager = playerInputManager;
            _boardManager = boardManager;
            _camera = camera;
            _characterSpawner = characterSpawner;
            _heroRow = heroRow;
            _boardSetting = boardSetting;
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

                var collisionProcessResult = CheckCollision();
                // heaven or hell? let's rock!
                switch (collisionProcessResult.CollidedObject)
                {
                    case Hero hero:
                    {
                        if (_heroRow.ContainsHero(hero))
                        {
                            // collided with player's hero
                            await ShowGameOverScreenAsync();
                            return;
                        }

                        MoveResultType moveResultType = _heroRow.TryMove(direction);
                        Debug.Log($"{nameof(GameState)} moveResultType({moveResultType})");
                    }
                        break;
                    case Enemy enemy:
                        break;
                    case Obstacle obstacle:
                        // do nothing, I guess
                        break;
                    case null: // no collision
                    {
                        MoveResultType moveResultType = _heroRow.TryMove(direction);
                        Debug.Log($"{nameof(GameState)} moveResultType({moveResultType})");
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                SpawnCharacters();
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
            _heroRow.SetupStartHero();
        }

        private CollisionCheckResult CheckCollision()
        {
            // TODO: implement this

            return new CollisionCheckResult()
            {
                CollidedObject = null,
            };
        }

        private void SpawnCharacters()
        {
        }

        private async UniTask ShowGameOverScreenAsync()
        {
            // TODO: implement this
            await UniTask.Delay(1);
        }

        void IDisposable.Dispose()
        {
            // consider cleaning up the board
        }
    }
}