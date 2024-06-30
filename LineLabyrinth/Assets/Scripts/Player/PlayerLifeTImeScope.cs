using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerLifeTimeScope : LifetimeScope
{
    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    PlayerProperty playerProperty;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(lineRenderer);
        builder.RegisterComponent(playerProperty);
    }
}
