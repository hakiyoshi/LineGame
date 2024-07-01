using Player;
using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using VContainer;

public class LineCurve : MonoBehaviour
{
    PlayerProperty property;

    [Inject]
    void Construct(PlayerProperty property)
    {
        this.property = property;
    }

    private void Start()
    {
        // 曲線更新
        property.LeftPoint.Subscribe(point =>
        {
            UpdateBezer();
        }).AddTo(this);

        property.RightPoint.Subscribe(point =>
        {
            UpdateBezer();
        }).AddTo(this);
    }

    /// <summary>
    /// ベジェ曲線を更新
    /// </summary>
    private void UpdateBezer()
    {
        //線の頂点数が違ったら調整する
        if(property.lineRenderer.positionCount != property.linePointCount)
        {
            property.lineRenderer.positionCount = property.linePointCount;
        }

        //最初と最後だけ座標がおかしかったので直接設定をする
        property.lineRenderer.SetPosition(0, property.LeftPointPosition);
        property.lineRenderer.SetPosition(property.linePointCount - 1, property.RightPointPosition);

        //点の座標を設定する
        for (int i = 1; i < property.linePointCount - 1; i++)
        {
            var bezier = Utility.BezierCurve.Culc3PointCurve(property.LeftPointPosition, property.RightPointPosition, property.ControlPoint, (float)i / (float)property.linePointCount);
            property.lineRenderer.SetPosition(i,
                new Vector3(bezier.x, bezier.y, -1.0f)
                );
        }
    }
}
