using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIeld
{
    /// <summary>
    /// フィールド上に配置される点オブジェクト
    /// プレイヤーの足場になる
    /// </summary>
    public class PointObject : MonoBehaviour
    {
        const int MaxLineObjectCount = 4;

        public const int UpLineObject = 0;
        public const int LeftLineObject = 1;
        public const int DownLineObject = 2;
        public const int RightLineObject = 3;

        /// <summary>
        /// 点に接続されている線オブジェクトの種類
        /// 線オブジェクトがない場合はNULL
        /// 1:上　2:右　3:下　4:左
        /// </summary>
        [field: SerializeField]
        public LineObject[] lineObject { get; private set; } = new LineObject[MaxLineObjectCount];
    }
}

