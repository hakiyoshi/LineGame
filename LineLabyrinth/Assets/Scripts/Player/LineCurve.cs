using Player;
using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using VContainer;

public class LineCurve : MonoBehaviour
{
    LineRenderer lineRenderer;
    PlayerProperty playerProperty;

    [Inject]
    void Construct(LineRenderer line, PlayerProperty property)
    {
        lineRenderer = line;
        playerProperty = property;
    }

    private void Start()
    {
        // 曲線更新
        playerProperty.LeftPoint.Subscribe(point =>
        {

        }).AddTo(this);

        playerProperty.RightPoint.Subscribe(point =>
        {

        }).AddTo(this);
    }

    private void UpdateBezer()
    {

    }
}
