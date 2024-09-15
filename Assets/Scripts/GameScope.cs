using System.Collections.Generic;
using Board;
using Characters;
using Input;
using Settings;
using State.Game;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

#nullable enable

public class CharacterData
{
    private readonly Hero _hero;

    public CharacterData(Hero hero)
    {
        _hero = hero;

        Debug.Log($"{nameof(CharacterData)} ctor hero({hero.GetHashCode()})");
    }
}

public class GameScope : LifetimeScope
{
    [SerializeField] private GameSetting gameSetting = null!;
    [SerializeField] private Hero heroPrefab = null!;
    [SerializeField] private CellComponent cellPrefab = null!;
    [SerializeField] private BoardManager boardManager = null!;
    [SerializeField] private Camera gameCamera = null!;

    private List<CharacterData> _testCharacterData = new List<CharacterData>();

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Input.PlayerInput>(Lifetime.Singleton);
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.Register<GameState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<PlayerInputManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        
        builder.RegisterInstance(gameCamera);

        // register board
        builder.RegisterInstance(gameSetting.BoardSetting);
        // would be better to let the board manager be it's own container, but haven't got enough time to research  
        builder.RegisterComponent(boardManager);
        builder.RegisterComponentInNewPrefab(cellPrefab, Lifetime.Transient);
        builder.Register<Board.Cell>(Lifetime.Transient).AsSelf();

        // remove this after testing
        builder.RegisterComponentInNewPrefab(heroPrefab, Lifetime.Transient);
        builder.Register<CharacterData>(Lifetime.Transient);
    }

    private void Start()
    {
        var gameManager =
            Container.Resolve<GameManager>(); // immediately resolve, just to test of a game manager is created
        Debug.Log($"{nameof(GameScope)} Start gameManager({gameManager.GetHashCode()})");

        Container.Resolve<PlayerInputManager>();

        // var boardSetting =  Container.Resolve<BoardSetting>();
        // var boardManager =  Container.Resolve<BoardManager>();
        // boardManager.SetBoardSetting(boardSetting);

        _testCharacterData.Add(Container.Resolve<CharacterData>());
        _testCharacterData.Add(Container.Resolve<CharacterData>());
    }
}