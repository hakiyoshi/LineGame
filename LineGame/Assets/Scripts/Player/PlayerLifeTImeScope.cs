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
    public class PlayerLifeTImeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerProperty>(Lifetime.Singleton);
        }
    }

}