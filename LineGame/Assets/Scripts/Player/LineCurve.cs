using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

public class LineCurve : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer = null;
    [SerializeField, Min(2)] int linePointCount = 2;
    [SerializeField] float lineHeight = 1.0f;



    /// <summary>
    /// 上方向ベクトル
    /// </summary>
    public Vector3 upVec { get; private set; }

    /// <summary>
    /// 線の中心座標
    /// </summary>
    public Vector3 centerPoint
    {
        get
        {
            return lineRenderer.GetPosition(lineRenderer.positionCount / 2);
        }
    }

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void OnValidate()
    {
        if(lineRenderer == null)
        {
            Debug.LogError("LineRendererがセットされていません")
            return;
        }

        //最初と最後のポイントを受け取る
        var startPoint = lineRenderer.GetPosition(0);
        var endPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        var lineVec = endPoint - startPoint;

        //上方向ベクトル計算
        upVec = Vector3.Cross(lineVec.normalized, new Vector3(0.0f, 0.0f, 1.0f)).normalized * lineHeight;

        //新しい座標ポイントを生成して適応する
        lineRenderer.positionCount = linePointCount;

        for (int i = 0; i < linePointCount; i++)
        {
            var centerPoint = (startPoint + (lineVec / 2)) + upVec;
            lineRenderer.SetPosition(i, CulcPoint(startPoint, endPoint, centerPoint, (float)i / (float)linePointCount));
        }
    }

    /// <summary>
    /// 引数tの位置の座標を求める
    /// </summary>
    /// <returns></returns>
    Vector3 CulcPoint(Vector3 start, Vector3 end, Vector3 center, float time)
    {
        var q1 = Vector3.Lerp(start, center, time);
        var q2 = Vector3.Lerp(center, end, time);
        return Vector3.Lerp(q1, q2, time);
    }
}
