using FIeld;
using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// プレイヤーの情報を保持するクラス
    /// VContainerからアクセスする
    /// </summary>
    public class PlayerProperty : MonoBehaviour
    {
        [SerializeField]
        private SerializableReactiveProperty<PointObject> leftPoint = new();

        /// <summary>
        /// 左足のポイントオブジェクト
        /// </summary>
        public ReadOnlyReactiveProperty<PointObject> LeftPoint => leftPoint;

        [SerializeField]
        private SerializableReactiveProperty<PointObject> rightPoint = new();

        /// <summary>
        /// 右足のポイントオブジェクト
        /// </summary>
        public ReadOnlyReactiveProperty<PointObject> RightPoint => rightPoint;

        /// <summary>
        /// 左足のポイントオブジェクトをセットする
        /// </summary>
        /// <param name="point"></param>
        public void SetLeftPoint(PointObject point)
        {
            leftPoint.Value = point;
        }

        /// <summary>
        /// 右足のポイントオブジェクトをセットする
        /// </summary>
        /// <param name="point"></param>
        public void SetRightPoint(PointObject point)
        {
            rightPoint.Value = point;
        }
    }
}

