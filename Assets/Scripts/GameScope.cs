using Board;
using Input;
using Settings;
using State.Game;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

#nullable enable

public class GameScope : LifetimeScope
{
    [SerializeField] private GameSetting gameSetting = null!;
    [SerializeField] private BoardManager boardManager = null!;
    [SerializeField] private Camera camera = null!;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Input.PlayerInput>(Lifetime.Singleton);
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.Register<GameState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<PlayerInputManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();

        builder.RegisterInstance(gameSetting.BoardSetting);
        // would be better to let the board manager be it's own container, but haven't got enough time to research  
        builder.RegisterComponent(boardManager);
        builder.RegisterInstance(camera);
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
    }
}