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

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Input.PlayerInput>(Lifetime.Singleton);
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.Register<GameState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        builder.Register<PlayerInputManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();

        builder.RegisterInstance(gameSetting.BoardSetting);
    }

    private void Start()
    {
        var gameManager =
            Container.Resolve<GameManager>(); // immediately resolve, just to test of a game manager is created
        Debug.Log($"{nameof(GameScope)} Start gameManager({gameManager.GetHashCode()})");

        Container.Resolve<PlayerInputManager>();
    }
}