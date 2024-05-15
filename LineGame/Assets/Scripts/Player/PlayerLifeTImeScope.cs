using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Player
{
    /// <summary>
    /// プレイヤーの依存関係を管理する
    /// </summary>
    public class PlayerLifeTimeScope : LifetimeScope
    {
        [SerializeField] private PlayerProperty playerProperty = null;
        [SerializeField] private LineRenderer lineRenderer = null;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Assert(playerProperty != null);
            Debug.Assert(lineRenderer != null);

            builder.RegisterComponent(playerProperty);
            builder.RegisterComponent(lineRenderer);
        }
    }

}