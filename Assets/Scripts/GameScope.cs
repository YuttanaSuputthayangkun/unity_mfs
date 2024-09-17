using System.Collections.Generic;
using Board;
using Characters;
using Data;
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

    private List<CharacterStats> _testCharacterData = new List<CharacterStats>();

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

        builder.RegisterInstance(gameSetting.SpawnSetting);
        builder.RegisterInstance(gameSetting.PrefabSetting);
        builder.RegisterInstance(gameSetting.DataSetting);
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

        _testCharacterData.Add(Container.Resolve<CharacterStats>());
        _testCharacterData.Add(Container.Resolve<CharacterStats>());
    }
}