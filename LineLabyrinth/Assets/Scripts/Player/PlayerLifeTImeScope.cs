using Input;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Player
{
    public class PlayerLifeTimeScope : LifetimeScope
    {
        PlayerInputAction input;

        [Header("プレイヤー設定")]
        [SerializeField]
        PlayerProperty property = null;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register(_ =>
            {
                return property;
            }, Lifetime.Singleton);

            builder.Register(_ =>
            {
                input = new();
                return input;
            }, Lifetime.Singleton);
        }

        private void Start()
        {
            property.CurrentLeftPosition = property.LeftPointPosition;
            property.CurrentRightPosition = property.RightPointPosition;

            property.ControlPoint = property.CulcDefaultControlPoint();

            input.Player.Enable();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (property == null)
            {
                return;
            }

            if (property.lineRenderer == null || property.RightPoint.CurrentValue == null || property.LeftPoint.CurrentValue == null)
            {
                return;
            }

            var vec = property.RightPointPosition - property.LeftPointPosition;

            var up = Vector3.Cross(new Vector3(0.0f, 0.0f, 1.0f), vec).normalized * property.LineHeight;
            var centerPoint = property.LeftPointPosition + (vec * 0.5f);

            var controlPoint = centerPoint + up;

            property.lineRenderer.positionCount = property.linePointCount;

            //最初と最後だけ座標がおかしかったので直接設定をする
            property.lineRenderer.SetPosition(0, new Vector3(property.LeftPointPosition.x, property.LeftPointPosition.y, -1.0f));
            property.lineRenderer.SetPosition(property.linePointCount - 1, new Vector3(property.RightPointPosition.x, property.RightPointPosition.y, -1.0f));

            for (int i = 1; i < property.linePointCount - 1; i++)
            {
                var bezier = Utility.BezierCurve.Culc3PointCurve(property.LeftPointPosition, property.RightPointPosition, controlPoint, (float)i / (float)property.linePointCount);
                property.lineRenderer.SetPosition(i,
                    new Vector3(bezier.x, bezier.y, -1.0f)
                    );
            }
        }
#endif
    }
}
