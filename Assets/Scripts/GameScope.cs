using Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable enable

public class GameScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<Input.PlayerInput>(Lifetime.Singleton);
        builder.Register<GameManager>(Lifetime.Singleton);
        builder.Register<PlayerInputManager>(Lifetime.Singleton).AsSelf().As<ITickable>();
    }

    private void Start()
    {
        var gameManager =  Container.Resolve<GameManager>();   // immediately resolve, just to test of a game manager is created
        Debug.Log($"{nameof(GameScope)} Start gameManager({gameManager.GetHashCode()})");

        Container.Resolve<PlayerInputManager>();
    }
}