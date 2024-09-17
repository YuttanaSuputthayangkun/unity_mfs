using System;
using System.Threading;
using Board;
using Characters;
using Cysharp.Threading.Tasks;
using Data;
using Input;
using Settings;
using UnityEngine;
using VContainer.Unity;

#nullable enable

namespace State.Game
{
    public class GameState : IState
        , IAsyncStartable
        , IDisposable
    {
        private struct CollisionCheckResult
        {
            public object? CollidedObject;
            public bool HasCollision => CollidedObject is not null;
        }

        private readonly PlayerInputManager _playerInputManager;
        private readonly BoardManager _boardManager;
        private readonly Camera _camera;
        private readonly HeroRow _heroRow;
        private readonly CharacterSpawnManager _characterSpawnManager;

        public StateType GetStateType() => StateType.GameState;

        public GameState(
            PlayerInputManager playerInputManager,
            BoardManager boardManager,
            Camera camera,
            HeroRow heroRow,
            CharacterSpawnManager characterSpawnManager
        )
        {
            _playerInputManager = playerInputManager;
            _boardManager = boardManager;
            _camera = camera;
            _heroRow = heroRow;
            _characterSpawnManager = characterSpawnManager;
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

            // one loop per action
            while (true)
            {
                cancellation.ThrowIfCancellationRequested();

                var direction = await _playerInputManager.WaitDirectionalInputAsync(cancellation);
                Debug.Log($"{nameof(GameState)} StartAsync direction({direction})");

                var collisionProcessResult = CheckCollision(direction);
                switch (collisionProcessResult.CollidedObject)
                {
                    case Hero hero:
                    {
                        // I ignore last hero because it should be moved along, so it's okay
                        if (_heroRow.ContainsHero(hero) && !_heroRow.IsLastHero(hero))
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
                        // heaven or hell? let's rock!
                        ProcessEnemyCollision();
                        SpawnCharacters();
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

        private CollisionCheckResult CheckCollision(Direction direciton)
        {
            var headCoordinate = _heroRow.First!.Coordinate;
            var nextCoordinate = headCoordinate.GetNeighbor(direciton);
            var getCellResult = _boardManager.GetCell(nextCoordinate);
            if (!getCellResult.IsFound)
            {
                // no collision, but won't be able to move anyway
                return new CollisionCheckResult();
            }
            
            

            return new CollisionCheckResult()
            {
                CollidedObject = null,
            };
        }

        private void SpawnCharacters()
        {
            // TODO: implement a way to specify spawn type, so we don't spawn an obstacle
            var spawnResult = _characterSpawnManager.RandomSpawn();
            Debug.Log($"SpawnCharacters spawnResult:\n{spawnResult}");
        }

        private async UniTask ShowGameOverScreenAsync()
        {
            // TODO: implement this
            await UniTask.Delay(1);
        }

        private void ProcessEnemyCollision()
        {
            // TODO: implement this
        }

        void IDisposable.Dispose()
        {
            // consider cleaning up the board
        }
    }
}