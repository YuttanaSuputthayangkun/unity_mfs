using System;
using System.Linq;
using Board;
using Characters;
using Data;
using Extensions;
using Input;
using Settings;
using State.Game;
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

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Input.PlayerInput>(Lifetime.Singleton);
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.Register<GameState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<PlayerInputManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<CharacterSpawner>(Lifetime.Singleton);
        builder.Register<HeroRow>(Lifetime.Singleton);
        builder.Register<CharacterSpawnManager>(Lifetime.Singleton);

        builder.RegisterInstance(gameCamera);

        // register board
        builder.RegisterInstance(gameSetting.BoardSetting);
        // would be better to let the board manager be it's own container, but haven't got enough time to research  
        builder.RegisterComponent(boardManager);
        builder.RegisterComponentInNewPrefab(cellPrefab, Lifetime.Transient);
        builder.Register<Board.Cell>(Lifetime.Transient).AsSelf();

        builder.RegisterInstance(gameSetting.SpawnSetting);
        builder.RegisterInstance(gameSetting.PrefabSetting);
        builder.RegisterInstance(gameSetting.DataSetting);

        builder.RegisterFactory<IReadOnlyCharacterData<HeroType>, Hero>(container =>
        {
            return heroData =>
            {
                var heroPrefabData =
                    gameSetting.PrefabSetting.HeroPrefabDataList.First(x => x.PrefabType == heroData.Type)
                    ?? throw new NotSupportedException("Cannot find hero prefab with type: {type}");
                var instantiated = container.Instantiate(heroPrefabData.Prefab);
                var newHero = new Hero(instantiated.GetComponent<CharacterComponent>(), heroData);
                return newHero;
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