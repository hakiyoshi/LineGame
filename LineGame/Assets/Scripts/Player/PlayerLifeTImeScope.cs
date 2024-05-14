using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Player
{
    /// <summary>
    /// �v���C���[�̈ˑ��֌W���Ǘ�����
    /// </summary>
    public class PlayerLifeTImeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerProperty>(Lifetime.Singleton);
        }
    }

}