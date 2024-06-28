using Field;
using NaughtyAttributes;
using R3;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// プレイヤーの情報を保持するクラス
    /// VContainerからアクセスする
    /// </summary>
    public class PlayerProperty : MonoBehaviour
    {
        [field: SerializeField]
        public LineRenderer lineRenderer { get; private set; }

        [SerializeField]
        private SerializableReactiveProperty<PointObject> leftPoint = new();

        /// <summary>
        /// 左足のポイント
        /// </summary>
        public ReadOnlyReactiveProperty<PointObject> LeftPoint => leftPoint;

        /// <summary>
        /// 左足のポイント座標
        /// </summary>
        public Vector3 LeftPointPosition => leftPoint.Value.transform.position;

        [SerializeField]
        private SerializableReactiveProperty<PointObject> rightPoint = new();

        /// <summary>
        /// 右足のポイント
        /// </summary>
        public ReadOnlyReactiveProperty<PointObject> RightPoint => rightPoint;

        /// <summary>
        /// 右足のポイント座標
        /// </summary>
        public Vector3 RightPointPosition => rightPoint.Value.transform.position;

        /// <summary>
        /// 上方向ベクトル
        /// </summary>
        [ReadOnly]
        public Vector3 UpVec;

        /// <summary>
        /// プレイヤーの線のポイント数
        /// </summary>
        [SerializeField, Min(2)] int linePointCount = 2;

        /// <summary>
        /// 線の高さ
        /// </summary>
        [SerializeField] float lineHeight = 1.0f;

        /// <summary>
        /// 左足のポイント座標をセットする
        /// </summary>
        /// <param name="point"></param>
        public void SetLeftPoint(PointObject point)
        {
            leftPoint.Value = point;
        }

        /// <summary>
        /// 右足のポイント座標をセットする
        /// </summary>
        /// <param name="point"></param>
        public void SetRightPoint(PointObject point)
        {
            rightPoint.Value = point;
        }

        private void Start()
        {
            UpVec = (RightPointPosition - LeftPointPosition).normalized * lineHeight;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (lineRenderer == null || rightPoint.Value == null || leftPoint.Value == null)
            {
                return;
            }

            var vec = RightPointPosition - LeftPointPosition;

            var up = Vector3.Cross(new Vector3(0.0f, 0.0f, 1.0f), vec).normalized * lineHeight;
            var centerPoint = LeftPointPosition + (vec * 0.5f);

            var controlPoint = centerPoint + up;

            lineRenderer.positionCount = linePointCount;

            for (int i = 0; i < linePointCount; i++)
            {
                var bezier = Utility.BezierCurve.Culc3PointCurve(LeftPointPosition, RightPointPosition, controlPoint, (float)i / (float)linePointCount);
                lineRenderer.SetPosition(i,
                    new Vector3(bezier.x, bezier.y, -1.0f)
                    );
            }
        }
#endif
    }
}

