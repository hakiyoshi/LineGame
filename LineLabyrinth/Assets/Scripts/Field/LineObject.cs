using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] PointObject[] points = new PointObject[MaxPointCount];

        private void Awake()
        {
            if (!TryGetComponent(out lineRenderer))
            {
                Debug.Assert(lineRenderer != null);
            }

            //TODO: ゲーム実行時にラインが引かれていなかったら下のコメントをコメントアウトする
            DrawLine();
        }

        private void OnGUI()
        {
            DrawLine();
        }

        private void DrawLine()
        {
            if (TryGetComponent(out LineRenderer lineRenderer) && points.Length == MaxPointCount)
            {
                lineRenderer.enabled = true;
                lineRenderer.positionCount = MaxPointCount;
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    lineRenderer.SetPosition(i, points[i].transform.position);
                }
                lineRenderer.SetPosition(1, points[1].transform.position);
            }
        }
    }

}