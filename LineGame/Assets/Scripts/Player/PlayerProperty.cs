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
    public class PlayerProperty
    {
        private ReactiveProperty<PointObject> leftPoint = new();
        public ReadOnlyReactiveProperty<PointObject> LeftPoint => leftPoint;

        private ReactiveProperty<PointObject> rightPoint = new();
        public ReadOnlyReactiveProperty<PointObject> RightPint => rightPoint;

        /// <summary>
        /// 左足に点オブジェクトをセットする
        /// </summary>
        /// <param name="point"></param>
        void SetLeftPoint(PointObject point)
        {
            leftPoint.Value = point;
        }

        /// <summary>
        /// 右足に点オブジェクトをセットする
        /// </summary>
        /// <param name="point"></param>
        void SetRightPint(PointObject point)
        {
            rightPoint.Value = point;
        }
    }
}

