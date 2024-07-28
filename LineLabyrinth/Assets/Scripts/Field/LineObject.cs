using System;
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
        public PointObject[] Points { get; private set; } = new PointObject[MaxPointCount];

        private void Awake()
        {
            if (!TryGetComponent(out lineRenderer))
            {
                Debug.Assert(lineRenderer != null);
            }
            
            DrawLine();
        }

        public void DrawLine()
        {
            if (TryGetComponent(out LineRenderer line) && Points.Length == MaxPointCount)
            {
                if (Points.Any(x => x == null) || Points.Distinct().Count() == 1)
                {
                    return;
                }

                line.enabled = true;
                line.positionCount = MaxPointCount;
                for (int i = 0; i <  line.positionCount; i++)
                {
                    var pointPosition = Points[i].transform.position;

                     line.SetPosition(i, pointPosition);

                    var sidePointPosition = Points[(i + 1) % MaxPointCount].transform.position;

                    if(Mathf.Approximately(sidePointPosition.x, pointPosition.x))
                    {
                        //上
                        if (sidePointPosition.y < pointPosition.y)
                        {
                            Points[i].LineObject[PointObject.DownLineObject] = this;
                        }
                        //下
                        else if (sidePointPosition.y >= pointPosition.y)
                        {
                            Points[i].LineObject[PointObject.UpLineObject] = this;
                        }
                    }
                    else if(Mathf.Approximately(sidePointPosition.y, pointPosition.y))
                    {
                        //左
                        if (sidePointPosition.x < pointPosition.x)
                        {
                            Points[i].LineObject[PointObject.RightLineObject] = this;
                        }
                        //右
                        else if (sidePointPosition.x >= pointPosition.x)
                        {
                            Points[i].LineObject[PointObject.LeftLineObject] = this;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 指定したポイントから伸びる壁のベクトルを計算する
        /// </summary>
        /// <param name="standardPoint">基準のポイント</param>
        /// <returns>指定したポイントから伸びる壁のベクトルを返す。
        /// 指定したポイントが見つからない場合は適当なポイントを基準にした壁のベクトルを返す</returns>
        public Vector3 CulcWallVec(PointObject standardPoint)
        {
            var index = Array.IndexOf(Points, standardPoint);
            if(index == -1)
            {
                return Points[0].transform.position - Points[1].transform.position;
            }
            return Points[(index + 1) % Points.Length].transform.position - Points[index].transform.position;
        }

        private void OnValidate()
        {
            DrawLine();
        }
    }

}