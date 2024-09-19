using System;
using System.Threading;
using Board;
using Characters;
using Characters.Interfaces;
using Cysharp.Threading.Tasks;
using Data;
using Input;
using Settings;
using UI;
using UnityEngine;
using VContainer.Unity;

#nullable enable

namespace State.Game
{
    public class GameState : IState
        , IAsyncStartable
        , IDisposable
    {
        private struct ProcessCollisionCheckResult
        {
            public bool ShouldContinueGame;
        }

        private struct ProcessEnemyCollisionResult
        {
            public bool IsHeroDead;
            public bool IsEnemyDead;
        }

        private readonly PlayerInputManager _playerInputManager;
        private readonly BoardManager _boardManager;
        private readonly Camera _camera;
        private readonly HeroRow _heroRow;
        private readonly CharacterSpawnManager _characterSpawnManager;
        private readonly NonPlayerCharacterList _nonPlayerCharacterList;
        private readonly GameOverScreen _gameOverScreen;
        private readonly CharacterDamageCalculator _damageCalculator;

        public StateType GetStateType() => StateType.GameState;

        public GameState(
            PlayerInputManager playerInputManager,
            BoardManager boardManager,
            Camera camera,
            HeroRow heroRow,
            CharacterSpawnManager characterSpawnManager,
            NonPlayerCharacterList nonPlayerCharacterList, // TODO: consider removing
            GameOverScreen gameOverScreen,
            CharacterDamageCalculator damageCalculator
        )
        {
            _playerInputManager = playerInputManager;
            _boardManager = boardManager;
            _camera = camera;
            _heroRow = heroRow;
            _characterSpawnManager = characterSpawnManager;
            _nonPlayerCharacterList = nonPlayerCharacterList;
            _gameOverScreen = gameOverScreen;
            _damageCalculator = damageCalculator;
        }

        UniTask IAsyncStartable.StartAsync(CancellationToken cancellation) => PlayAsync(cancellation);

        public async UniTask PlayAsync(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            SetupScene();
            SetupStartHero();
            RandomSpawnStartHeroes();
            RandomSpawnStartEnemies();

            // one loop per action
            while (true)
            {
                cancellation.ThrowIfCancellationRequested();

                var direction = await _playerInputManager.WaitDirectionalInputAsync(cancellation);
                Debug.Log($"{nameof(GameState)} StartAsync direction({direction})");

                var collisionProcessResult = ProcessCollision(direction);
                if (!collisionProcessResult.ShouldContinueGame)
                {
                    await ShowGameOverScreenAsync(cancellation);
                    ReloadScene();
                    return;
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

        private ProcessCollisionCheckResult ProcessCollision(
            Direction direction
        )
        {
            var head = _heroRow.GetFirst()!;
            var headCoordinate = head.GetBoardCoordinate();
            var nextCoordinate = headCoordinate!.Value.GetNeighbor(direction);
            var getCellResult = _boardManager.GetCell(nextCoordinate);

            Debug.Log($"check collision, found character?: {getCellResult.CellData?.Character}");

            switch (getCellResult.CellData?.Character)
            {
                case Hero collidedHero:
                {
                    if (collidedHero.IsPlayerCharacter())
                    {
                        int? heroIndex = _heroRow.GetHeroIndex(collidedHero);
                        // we will not show game over when player try to move back
                        // but will do nothing and let the game continues
                        if (heroIndex == 1)
                        {
                            // TODO: consider passing enum to signify the status
                            return new ProcessCollisionCheckResult() { ShouldContinueGame = true };
                        }
                        else
                        {
                            // collided with player's hero
                            return new ProcessCollisionCheckResult() { ShouldContinueGame = false };
                        }
                    }

                    // TODO: might want to handle tail collision explicitly, in case we want to be able to move to the tip

                    Debug.Log($"{nameof(GameState)} collided with non player hero({collidedHero})");

                    var originalTailCoordinate = _heroRow.GetLast()!.GetBoardCoordinate()!.Value;

                    // remove out of the way first to avoid collision
                    _boardManager.RemoveCharacter(collidedHero.GetBoardCoordinate()!.Value);

                    MoveResultType moveResultType = _heroRow.TryMove(direction);
                    Debug.Log($"{nameof(GameState)} moveResultType({moveResultType})");

                    _heroRow.AddLast(originalTailCoordinate!, collidedHero);

                    SpawnCharacters(CharacterType.Hero);

                    break;
                }
                case Enemy enemy:
                {
                    var result = ProcessEnemyCollision(_heroRow.GetFirst()!, enemy);

                    if (result.IsHeroDead)
                    {
                        var originalHeadCoordinate = _heroRow.GetFirst()!.GetBoardCoordinate()!.Value;

                        var deadHero = _heroRow.RemoveFirst();
                        _boardManager.RemoveCharacter(deadHero.GetBoardCoordinate()!.Value);
                        deadHero.Remove(); // physically remove

                        if (_heroRow.HeroCount == 0)
                        {
                            return new ProcessCollisionCheckResult() { ShouldContinueGame = false };
                        }

                        var newHeadCoordinate = _heroRow.GetFirst()!.GetBoardCoordinate()!.Value;

                        // move to replace the previous head
                        var directionForNewHead =
                            newHeadCoordinate.TryGetNeighborDirection(originalHeadCoordinate)!.Value;
                        _heroRow.TryMove(directionForNewHead);
                    }

                    if (result.IsEnemyDead)
                    {
                        _boardManager.RemoveCharacter(enemy.GetBoardCoordinate()!.Value);
                        enemy.Remove(); // physically remove

                        SpawnCharacters(CharacterType.Enemy);
                    }
                }
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
                    throw new NotImplementedException();
            }

            return new ProcessCollisionCheckResult() { ShouldContinueGame = true };
        }

        // use random count if not specified
        private void SpawnCharacters(CharacterType characterType, int? count = null)
        {
            var spawnResult = _characterSpawnManager.RandomSpawnOnEmptyCells(characterType);
            Debug.Log($"SpawnCharacters spawnResult:\n{spawnResult}");
        }

        private void RandomSpawnStartHeroes()
        {
            var spawnResult = _characterSpawnManager.RandomSpawnStartHeroes();
            Debug.Log($"RandomSpawnStartHeroes spawnResult:\n{spawnResult}");
        }
        
        private void RandomSpawnStartEnemies()
        {
            var spawnResult = _characterSpawnManager.RandomSpawnStartEnemies();
            Debug.Log($"RandomSpawnStartEnemies spawnResult:\n{spawnResult}");
        }

        private async UniTask ShowGameOverScreenAsync(CancellationToken cancellationToken)
        {
            _gameOverScreen.SetActive(true);
            Debug.Log($"GAME OVER!");

            // TODO: add more input
            await _playerInputManager.WaitDirectionalInputAsync(cancellationToken);
        }

        private void ReloadScene()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var sceneName = scene.name!;
            _ = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        private ProcessEnemyCollisionResult ProcessEnemyCollision(Hero hero, Enemy enemy)
        {
            int damageToEnemy = _damageCalculator.CalculateDamage(
                hero,
                hero.HeroType,
                enemy,
                enemy.EnemyType
            ).Damage;

            int damageToHero = _damageCalculator.CalculateDamage(
                enemy,
                enemy.EnemyType,
                hero,
                hero.HeroType
            ).Damage;

            var newHeroStats = new CharacterStats(hero);
            newHeroStats.health -= damageToHero;
            Debug.Log($"ProcessEnemyCollision damageToHero({damageToHero}) heroStats({new CharacterStats(hero)}) newHeroStats({newHeroStats})");
            hero.SetCharacterStats(newHeroStats);

            var newEnemyStats = new CharacterStats(enemy);
            newEnemyStats.health -= damageToEnemy;
            Debug.Log($"ProcessEnemyCollision damageToEnemy({damageToEnemy}) enemyStats({new CharacterStats(enemy)}) newEnemyStats({newEnemyStats})");
            enemy.SetCharacterStats(newEnemyStats);

            return new ProcessEnemyCollisionResult()
            {
                IsHeroDead = newHeroStats.health <= 0,
                IsEnemyDead = newEnemyStats.health <= 0,
            };
        }

        void IDisposable.Dispose()
        {
            // consider cleaning up the board
        }
    }
}