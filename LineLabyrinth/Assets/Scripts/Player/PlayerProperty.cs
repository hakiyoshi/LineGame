using Field;
using R3;
using System;
using Unity.Collections;
using UnityEngine;
using VContainer.Unity;

namespace Player
{
    [Serializable]
    class PlayerProperty
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

        /// <summary>
        /// 左足の現在の座標
        /// </summary>
        [ReadOnly]
        public Vector3 CurrentLeftPosition = Vector3.zero;

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
        /// 右足の現在の座標
        /// </summary>
        [ReadOnly]
        public Vector3 CurrentRightPosition = Vector3.zero;

        /// <summary>
        /// ベジェ曲線の曲線を制御する座標設定
        /// </summary>
        [SerializeField]
        public Vector3 ControlPoint = Vector3.zero;

        /// <summary>
        /// 線の高さ
        /// </summary>
        [SerializeField]
        public float LineHeight = 1.0f;

        [SerializeField, Min(2)]
        public int linePointCount = 15;

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

        public Vector3 CulcDefaultControlPoint()
        {
            var vec = RightPointPosition - LeftPointPosition;
            var up = Vector3.Cross(new Vector3(0.0f, 0.0f, 1.0f), vec).normalized * LineHeight;
            var centerPoint = LeftPointPosition + (vec * 0.5f);
            return centerPoint + up;
        }
    }
}

