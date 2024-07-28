using UnityEngine;
using VContainer;

namespace Player
{
    public class LineCurve : MonoBehaviour
    {
        PlayerProperty property;

        [Inject]
        void Construct(PlayerProperty property)
        {
            this.property = property;
        }

        private void Start()
        {
            property.CurrentLeftPosition = property.LeftPointPosition;
            property.CurrentRightPosition = property.RightPointPosition;
        }

        private void Update()
        {
            UpdateBezer();
        }

        /// <summary>
        /// ベジェ曲線を更新
        /// </summary>
        private void UpdateBezer()
        {
            //線の頂点数が違ったら調整する
            if(property.PlayerLineRenderer.positionCount != property.LinePointCount)
            {
                property.PlayerLineRenderer.positionCount = property.LinePointCount;
            }

            //最初と最後だけ座標がおかしかったので直接設定をする
            property.PlayerLineRenderer.SetPosition(0, new Vector3(property.CurrentLeftPosition.x, property.CurrentLeftPosition.y, -1.0f));
            property.PlayerLineRenderer.SetPosition(property.LinePointCount - 1, new Vector3(property.CurrentRightPosition.x, property.CurrentRightPosition.y, -1.0f));

            //点の座標を設定する
            for (int i = 1; i < property.LinePointCount - 1; i++)
            {
                var bezier = Utility.BezierCurve.Culc3PointCurve(property.CurrentLeftPosition, property.CurrentRightPosition, property.ControlPoint, (float)i / property.LinePointCount);
                property.PlayerLineRenderer.SetPosition(i,
                    new Vector3(bezier.x, bezier.y, -1.0f)
                );
            }
        }
    }
}
