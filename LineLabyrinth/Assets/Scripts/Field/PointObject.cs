﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Field
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
        [field: SerializeField, Header("1:上　2:右　3:下　4:左")]
        public LineObject[] lineObject { get; private set; } = new LineObject[MaxLineObjectCount];

#if UNITY_EDITOR
        [Header("線接続")]
        [SerializeField]
        GameObject lineObjectParent;

        [SerializeField]
        GameObject lineObjectPrefab;

        [SerializeField]
        PointObject joinPoint;

        static int index = 0;

        private void OnValidate()
        {
            if(joinPoint != null)
            {
                if (lineObject == null || lineObject.Length < MaxLineObjectCount)
                {
                    lineObject = new LineObject[MaxLineObjectCount];
                }

                if (Mathf.Approximately(joinPoint.transform.position.x, transform.position.x))
                {
                    //上
                    if (joinPoint.transform.position.y < transform.position.y)
                    {
                        CreateLineObject(joinPoint, out var obj, out var line);
                        lineObject[PointObject.DownLineObject] = line;
                    }
                    //下
                    else if (joinPoint.transform.position.y >= transform.position.y)
                    {
                        CreateLineObject(joinPoint, out var obj, out var line);
                        lineObject[PointObject.UpLineObject] = line;
                    }
                }
                else if (Mathf.Approximately(joinPoint.transform.position.y, transform.position.y))
                {
                    //左
                    if (joinPoint.transform.position.x < transform.position.x)
                    {
                        CreateLineObject(joinPoint, out var obj, out var line);
                        lineObject[PointObject.RightLineObject] = line;
                    }
                    //右
                    else if (joinPoint.transform.position.x >= transform.position.x)
                    {
                        CreateLineObject(joinPoint, out var obj, out var line);
                        lineObject[PointObject.LeftLineObject] = line;
                    }
                }
            }

            joinPoint = null;
        }

        void CreateLineObject(PointObject joinPoint, out GameObject obj, out LineObject line)
        {
            obj = Instantiate(lineObjectPrefab, lineObjectParent.transform);
            obj.gameObject.name = $"Line({index})";
            line = obj.GetComponent<LineObject>();
            if (line == null)
            {
                Debug.LogError("LineObjectが設定されていません");
                return;
            }
            line.points[0] = this;
            line.points[1] = joinPoint;
            line.DrawLine();
            index++;
        }
#endif
    }
}

