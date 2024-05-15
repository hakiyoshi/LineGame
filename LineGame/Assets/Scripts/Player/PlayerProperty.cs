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
        public ReadOnlyReactiveProperty<PointObject> LeftPoint => leftPoint;

        [SerializeField]
        private SerializableReactiveProperty<PointObject> rightPoint = new();
        public ReadOnlyReactiveProperty<PointObject> RightPoint => rightPoint;

        public void SetLeftPoint(PointObject point)
        {
            leftPoint.Value = point;
        }

        public void SetRIghtPoint(PointObject point)
        {
            rightPoint.Value = point;
        }
    }
}

