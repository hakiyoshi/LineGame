using DG.Tweening;
using Field;
using Input;
using Player;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class PlayerMove : MonoBehaviour
{
    PlayerProperty property;
    PlayerInputAction input;
    Collider2D collider2d;

    /// <summary>
    /// コントロールポイントの中心からの移動量
    /// </summary>
    Vector3 controlPointMoveOffset = Vector3.zero;

    /// <summary>
    /// 移動幅
    /// </summary>
    [SerializeField]
    float moveDistance = 1.0f;

    [SerializeField]
    float controllPointMoveSpeed = 0.2f;

    [SerializeField]
    float moveLegSeconds = 0.2f;

    Tweener leftLegMove = null;
    Tweener rightLegMove = null;

    [Inject]
    void Construct(PlayerProperty property, PlayerInputAction input, Collider2D collider)
    {
        this.property = property;
        this.input = input;
        this.collider2d = collider;
    }

    private void Start()
    {
        collider2d.offset = CulcOffsetDefaultPosition();
    }

    private void Update()
    {
        MoveControlPoint();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        MoveLeg(collider);
    }

    /// <summary>
    /// コントロールポイント制御
    /// </summary>
    void MoveControlPoint()
    {
        var moveInput = input.Player.Move.ReadValue<Vector2>();

        var defaultControlPoint = property.CulcDefaultControlPoint();

        var move = new Vector3(moveInput.x, moveInput.y, 0.0f) * moveDistance;

        //指定の座標まで徐々に移動させる
        DOVirtual.Vector3(property.ControlPoint, defaultControlPoint + move, controllPointMoveSpeed, x => property.ControlPoint = x);

        //移動判定の当たり判定のオフセットを移動させる
        DOVirtual.Vector3(collider2d.offset, CulcOffsetDefaultPosition() + move, controllPointMoveSpeed, x => collider2d.offset = x);
    }

    Vector3 CulcOffsetDefaultPosition()
    {
        return property.CurrentLeftPosition + ((property.CurrentRightPosition - property.CurrentLeftPosition) * 0.5f);
    }

    /// <summary>
    /// 足の現在座標の移動
    /// </summary>
    /// <param name="collider"></param>
    void MoveLeg(Collider2D collider)
    {
        if (collider.TryGetComponent<PointObject>(out var targetPoint))
        {
            //自分の足のポイントオブジェクトが同じ場合は無視する
            if (property.LeftPoint.CurrentValue == targetPoint || property.RightPoint.CurrentValue == targetPoint)
            {
                return;
            }

            //近い足とポイントオブジェクトを入れ替える
            if (Vector3.Distance(property.LeftPoint.CurrentValue.transform.position, targetPoint.transform.position) <=
                Vector3.Distance(property.RightPoint.CurrentValue.transform.position, targetPoint.transform.position))
            {
                if(IsHitWall(targetPoint, property.LeftPoint.CurrentValue))
                {
                    return;
                }

                var leftPointObject = property.LeftPoint.CurrentValue;
                property.SetLeftPoint(targetPoint);
                property.SetRightPoint(leftPointObject);
            }
            else
            {
                if (IsHitWall(targetPoint, property.RightPoint.CurrentValue))
                {
                    return;
                }

                var rightPointObject = property.RightPoint.CurrentValue;
                property.SetRightPoint(targetPoint);
                property.SetLeftPoint(rightPointObject);
            }

            //複数のDoTweenが動かないようにその場で止める
            leftLegMove?.Complete();
            rightLegMove?.Complete();

            //現在の足の座標を変更する
            DOVirtual.Vector3(property.CurrentLeftPosition, property.LeftPointPosition, moveLegSeconds, x => property.CurrentLeftPosition = x);
            DOVirtual.Vector3(property.CurrentRightPosition, property.RightPointPosition, moveLegSeconds, x => property.CurrentRightPosition = x);
        }
    }

    bool IsHitWall(PointObject targetPoint, PointObject legPoint)
    {
        var controlPointVec = (targetPoint.transform.position - property.ControlPoint).normalized;

        foreach (var lineObject in legPoint.lineObject)
        {
            //移動先が線とつながっていない場合は無視する
            if(lineObject == null)
            {
                continue;
            }

            var wallVec = lineObject.points[0].transform.position - lineObject.points[1].transform.position;
            var targetPointVec = (legPoint.transform.position - targetPoint.transform.position).normalized;
            var controlPointCross = Vector3.Cross(wallVec, controlPointVec).normalized;
            var targetCross = Vector3.Cross(wallVec, targetPointVec).normalized;

            Debug.Log($"{lineObject.gameObject.name} コントロール{controlPointCross.z} ターゲット{Mathf.Sign(targetCross.z)}");

            //壁のベクトルと移動先ベクトルが同じ場合無視する
            if(Mathf.Approximately(targetCross.z, 0.0f))
            {
                continue;
            }

            //壁のベクトルを挟んで左右にある場合間に壁があると判断する
            if(Mathf.Sign(controlPointCross.z) !=  Mathf.Sign(targetCross.z))
            {
                return true;
            }
        }

        return false;
    }
}
