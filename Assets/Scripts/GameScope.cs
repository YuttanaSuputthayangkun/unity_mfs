using System;
using System.Linq;
using Board;
using Characters;
using Data;
using Extensions;
using Input;
using Settings;
using State.Game;
using UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

public class GameScope : LifetimeScope
{
    [SerializeField] private GameSetting gameSetting = null!;
    [SerializeField] private CellComponent cellPrefab = null!;
    [SerializeField] private BoardManager boardManager = null!;
    [SerializeField] private Camera gameCamera = null!;
    [SerializeField] private CharacterPoolComponent characterPool = null!;
    [SerializeField] private GameOverScreenComponent gameGameOverScreenComponent = null!;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Input.PlayerInput>(Lifetime.Singleton);
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.Register<GameState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<PlayerInputManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<CharacterSpawner>(Lifetime.Singleton);
        builder.Register<HeroRow>(Lifetime.Singleton);
        builder.Register<CharacterSpawnManager>(Lifetime.Singleton);
        builder.Register<NonPlayerCharacterList>(Lifetime.Singleton);
        builder.Register<MoveCharacterHandler>(Lifetime.Singleton);
        builder.Register<LocateCharacterHandler>(Lifetime.Singleton);
        builder.Register<CharacterDamageCalculator>(Lifetime.Singleton);

        builder.RegisterInstance(gameCamera);

        // setup pool
        builder.RegisterInstance(characterPool);
        builder.Register<CharacterPool>(Lifetime.Singleton);
        builder.Register<RemoveCharacterHandler>(Lifetime.Singleton);

        // register board
        builder.RegisterInstance(gameSetting.BoardSetting);
        // would be better to let the board manager be it's own container, but haven't got enough time to research  
        builder.RegisterComponent(boardManager);
        builder.RegisterComponentInNewPrefab(cellPrefab, Lifetime.Transient);
        builder.Register<Board.Cell>(Lifetime.Transient).AsSelf();

        // setttings
        builder.RegisterInstance(gameSetting.SpawnSetting);
        builder.RegisterInstance(gameSetting.PrefabSetting);
        builder.RegisterInstance(gameSetting.DataSetting);
        builder.RegisterInstance(gameSetting.DamageCalculationSetting);

        // UI
        builder.RegisterInstance(gameGameOverScreenComponent);
        builder.Register<GameOverScreen>(Lifetime.Singleton);

        builder.RegisterFactory<IReadOnlyCharacterData<HeroType>, Hero>(container =>
        {
            return heroData =>
            {
                var pool = container.Resolve<CharacterPool>();
                if (pool.Pop(CharacterType.Hero) is Hero poolCharacter)
                {
                    // TODO: consider moving this process into Dispose or something
                    poolCharacter.SetCharacterStats(heroData.Stats);
                    poolCharacter.SetNumber(null);
                    return poolCharacter;
                }

                var heroPrefabData =
                    gameSetting.PrefabSetting.HeroPrefabDataList.First(x => x.PrefabType == heroData.Type)
                    ?? throw new NotSupportedException("Cannot find hero prefab with type: {type}");
                var instantiated = container.Instantiate(heroPrefabData.Prefab);
                var moveHandler = container.Resolve<MoveCharacterHandler>();
                var locateCharacterHandler = container.Resolve<LocateCharacterHandler>();
                var removeCharacterHandler = container.Resolve<RemoveCharacterHandler>();
                var newHero = new Hero(
                    instantiated.GetComponent<CharacterComponent>(),
                    heroData,
                    moveHandler,
                    locateCharacterHandler,
                    removeCharacterHandler,
                    (h) =>
                    {
                        var heroRow = container.Resolve<HeroRow>();
                        return heroRow.ContainsHero((Hero)h);
                    }
                );
                return newHero;
            };
        }, Lifetime.Transient);

        builder.RegisterFactory<IReadOnlyCharacterData<EnemyType>, Enemy>(container =>
        {
            return enemyData =>
            {
                var pool = container.Resolve<CharacterPool>();
                if (pool.Pop(CharacterType.Enemy) is Enemy poolCharacter)
                {
                    // TODO: consider moving this process into Dispose or something
                    poolCharacter.SetCharacterStats(enemyData.Stats);
                    return poolCharacter;
                }

                var enemyPrefabData =
                    gameSetting.PrefabSetting.EnemyPrefabDataList.First(x => x.PrefabType == enemyData.Type)
                    ?? throw new NotSupportedException("Cannot find enemy prefab with type: {type}");
                var instantiated = container.Instantiate(enemyPrefabData.Prefab);
                var moveHandler = container.Resolve<MoveCharacterHandler>();
                var locateCharacterHandler = container.Resolve<LocateCharacterHandler>();
                var removeCharacterHandler = container.Resolve<RemoveCharacterHandler>();
                var enemy = new Enemy(
                    instantiated.GetComponent<CharacterComponent>(),
                    enemyData,
                    moveHandler,
                    removeCharacterHandler,
                    locateCharacterHandler
                );
                return enemy;
            };
        }, Lifetime.Transient);
    }

    private void Start()
    {
        var gameManager =
            Container.Resolve<GameManager>(); // immediately resolve, just to test of a game manager is created
        Debug.Log($"{nameof(GameScope)} Start gameManager({gameManager.GetHashCode()})");

        // var heroData = gameSetting.DataSetting.HeroDataList[0] ?? throw new NullReferenceException();
        // var factory = Container.Resolve<Func<IReadOnlyCharacterData<HeroType>, Hero>>();
        // var newHero = factory.Invoke(heroData);
        // Debug.Log($"newHero:\n{newHero}");
    }
}