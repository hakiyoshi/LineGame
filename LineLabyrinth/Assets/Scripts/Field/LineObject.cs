using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Field
{
    /// <summary>
    /// フィールド上に線を表現するオブジェクト
    /// 接続されている点2つを管理している
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class LineObject : MonoBehaviour
    {
        LineRenderer lineRenderer = null;

        const int MaxPointCount = 2;

        /// <summary>
        /// 線をつなぐ点オブジェクト
        /// </summary>
        [field: SerializeField]
        public PointObject[] points { get; private set; } = new PointObject[MaxPointCount];

        private void Awake()
        {
            if (!TryGetComponent(out lineRenderer))
            {
                Debug.Assert(lineRenderer != null);
            }

            //TODO: ゲーム実行時にラインが引かれていなかったら下のコメントをコメントアウトする
            DrawLine();
        }

        private void DrawLine()
        {
            if (TryGetComponent(out LineRenderer lineRenderer) && points.Length == MaxPointCount)
            {
                if (points.Any(x => x == null) || points.Distinct().Count() == 1)
                {
                    return;
                }

                lineRenderer.enabled = true;
                lineRenderer.positionCount = MaxPointCount;
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    var pointPosition = points[i].transform.position;

                    lineRenderer.SetPosition(i, pointPosition);

                    var sidePointPosition = points[(i + 1) % MaxPointCount].transform.position;

                    if(Mathf.Approximately(sidePointPosition.x, pointPosition.x))
                    {
                        //上
                        if (sidePointPosition.y < pointPosition.y)
                        {
                            points[i].lineObject[PointObject.DownLineObject] = this;
                        }
                        //下
                        else if (sidePointPosition.y >= pointPosition.y)
                        {
                            points[i].lineObject[PointObject.UpLineObject] = this;
                        }
                    }
                    else if(Mathf.Approximately(sidePointPosition.y, pointPosition.y))
                    {
                        //左
                        if (sidePointPosition.x < pointPosition.x)
                        {
                            points[i].lineObject[PointObject.RightLineObject] = this;
                        }
                        //右
                        else if (sidePointPosition.x >= pointPosition.x)
                        {
                            points[i].lineObject[PointObject.LeftLineObject] = this;
                        }
                    }
                }
            }
        }

        private void OnValidate()
        {
            DrawLine();
        }
    }

}