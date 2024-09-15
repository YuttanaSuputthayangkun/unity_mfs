using UnityEngine;
using VContainer;
using VContainer.Unity;

#nullable disable

public class GameScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<GameManager>(Lifetime.Singleton);
    }

    private void Start()
    {
        var gameManager =  Container.Resolve<GameManager>();   // immediately resolve, just to test of a game manager is created
        Debug.Log($"{nameof(GameScope)} Start gameManager({gameManager.GetHashCode()})");
    }
}