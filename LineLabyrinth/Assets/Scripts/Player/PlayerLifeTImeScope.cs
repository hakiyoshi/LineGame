using Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Player
{
    public class PlayerLifeTimeScope : LifetimeScope
    {
        [SerializeField]
        Collider2D collider2d = null;

        PlayerInputAction input;

        [Header("プレイヤー設定")]
        [SerializeField]
        PlayerProperty property = null;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register(_ => property, Lifetime.Singleton);

            builder.Register(_ =>
            {
                input = new();
                return input;
            }, Lifetime.Singleton);

            builder.RegisterComponent(collider2d);
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

            if (property.PlayerLineRenderer == null || property.RightPoint.CurrentValue == null || property.LeftPoint.CurrentValue == null)
            {
                return;
            }

            var vec = property.RightPointPosition - property.LeftPointPosition;

            var up = Vector3.Cross(new Vector3(0.0f, 0.0f, 1.0f), vec).normalized * property.LineHeight;
            var centerPoint = property.LeftPointPosition + (vec * 0.5f);

            var controlPoint = centerPoint + up;

            property.PlayerLineRenderer.positionCount = property.LinePointCount;

            //最初と最後だけ座標がおかしかったので直接設定をする
            property.PlayerLineRenderer.SetPosition(0, new Vector3(property.LeftPointPosition.x, property.LeftPointPosition.y, -1.0f));
            property.PlayerLineRenderer.SetPosition(property.LinePointCount - 1, new Vector3(property.RightPointPosition.x, property.RightPointPosition.y, -1.0f));

            for (int i = 1; i < property.LinePointCount - 1; i++)
            {
                var bezier = Utility.BezierCurve.Culc3PointCurve(property.LeftPointPosition, property.RightPointPosition, controlPoint, (float)i / property.LinePointCount);
                property.PlayerLineRenderer.SetPosition(i,
                    new Vector3(bezier.x, bezier.y, -1.0f)
                    );
            }
        }
#endif
    }
}
