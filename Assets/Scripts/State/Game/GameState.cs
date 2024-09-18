using System;
using System.Threading;
using Board;
using Characters;
using Characters.Interfaces;
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
            public BoardCoordinate Coordinate;
            public ICharacter? Character;
            public bool IsPlayerCharacter;
            public bool HasCollision => Character is not null;
        }

        private readonly PlayerInputManager _playerInputManager;
        private readonly BoardManager _boardManager;
        private readonly Camera _camera;
        private readonly HeroRow _heroRow;
        private readonly CharacterSpawnManager _characterSpawnManager;
        private readonly NonPlayerCharacterList _nonPlayerCharacterList;

        public StateType GetStateType() => StateType.GameState;

        public GameState(
            PlayerInputManager playerInputManager,
            BoardManager boardManager,
            Camera camera,
            HeroRow heroRow,
            CharacterSpawnManager characterSpawnManager,
            NonPlayerCharacterList nonPlayerCharacterList
        )
        {
            _playerInputManager = playerInputManager;
            _boardManager = boardManager;
            _camera = camera;
            _heroRow = heroRow;
            _characterSpawnManager = characterSpawnManager;
            _nonPlayerCharacterList = nonPlayerCharacterList;
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
                switch (collisionProcessResult.Character)
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

                        // TODO: call set coordinate from character instead
                        // _boardManager.SetCellCharacterType(collisionProcessResult.Coordinate, null);

                        var originalTailCoordinate = _heroRow.GetLast()!.GetBoardCoordinate()!.Value;

                        MoveResultType moveResultType = _heroRow.TryMove(direction);
                        Debug.Log($"{nameof(GameState)} moveResultType({moveResultType})");

                        _heroRow.AddLast(originalTailCoordinate!);

                        // add to tail
                    }
                        break;
                    case Enemy enemy:
                        // heaven or hell? let's rock!
                        ProcessEnemyCollision();
                        SpawnCharacters();

                        // TODO: move

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
            Debug.Log($"setupBoardResult({setupBoardResult})");

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

        private CollisionCheckResult CheckCollision(Direction direction)
        {
            var head = _heroRow.GetFirst()!;
            var headCoordinate = head.GetBoardCoordinate();
            var nextCoordinate = headCoordinate!.Value.GetNeighbor(direction);
            var getCellResult = _boardManager.GetCell(nextCoordinate);
            var result = new CollisionCheckResult() { Coordinate = nextCoordinate };
            if (!getCellResult.IsFound)
            {
                // no collision, but won't be able to move anyway
                return result;
            }

            switch (getCellResult.CellData?.Character)
            {
                case Hero hero:
                {
                    // bool isPlayerCharacter = _heroRow.ContainsHero(hero);
                    throw new NotImplementedException();
                }
                case null:
                    return result;
                default:
                    throw new NotImplementedException();
            }
        }

        private void SpawnCharacters()
        {
            // TODO: implement a way to specify spawn type, so we don't spawn an obstacle
            var spawnResult = _characterSpawnManager.RandomSpawnOnEmptyCells();
            Debug.Log($"SpawnCharacters spawnResult:\n{spawnResult}");
        }

        private async UniTask ShowGameOverScreenAsync()
        {
            // TODO: implement this
            Debug.Log($"GAME OVER!");
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