using DG.Tweening;
using Field;
using Input;
using Player;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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

    Tweener controlPointMove = null;
    Tweener colliderMove = null;

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

        controlPointMove?.Kill();
        colliderMove?.Kill();

        //指定の座標まで徐々に移動させる
        controlPointMove = DOVirtual.Vector3(property.ControlPoint, defaultControlPoint + move, controllPointMoveSpeed, x => property.ControlPoint = x);

        //移動判定の当たり判定のオフセットを移動させる
        colliderMove = DOVirtual.Vector3(collider2d.offset, CulcOffsetDefaultPosition() + move, controllPointMoveSpeed, x => collider2d.offset = x);
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
        //移動中は移動判定をしない
        if((leftLegMove?.active ?? false) || (rightLegMove?.active ?? false))
        {
            return;
        }

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
            leftLegMove = DOVirtual.Vector3(property.CurrentLeftPosition, property.LeftPointPosition, moveLegSeconds, x => property.CurrentLeftPosition = x);
            rightLegMove = DOVirtual.Vector3(property.CurrentRightPosition, property.RightPointPosition, moveLegSeconds, x => property.CurrentRightPosition = x);
        }
    }

    /// <summary>
    /// 全方向の壁判定
    /// </summary>
    /// <param name="targetPoint">移動先のポイント</param>
    /// <param name="legPoint">足のポイント</param>
    /// <returns>壁に阻まれてたらTrue</returns>
    bool IsHitWall(PointObject targetPoint, PointObject legPoint)
    {
        for(int i = 0; i < legPoint.lineObject.Length; i++)
        {
            if (IsHitWall(legPoint.lineObject[i], targetPoint, legPoint))
            {
                //連続して隣接している壁が2か所空いていた場合壁に阻まれていない判定にする
                int rightCount = 0;
                int leftCount = 0;
                const int roopCount = 2;
                for (int j = 1; j < roopCount + 1; j++)
                {
                    var rightSideLine = legPoint.lineObject[((i - j) + legPoint.lineObject.Length) % legPoint.lineObject.Length];
                    var leftSideLine = legPoint.lineObject[(i + j) % legPoint.lineObject.Length];

                    //壁が無い場合カウントを増やす
                    if (rightSideLine == null)
                    {
                        rightCount++;
                    }

                    if(leftSideLine == null)
                    {
                        leftCount++;
                    }
                }

                //壁が無い場合はヒットしていない判定にする
                if(rightCount >= roopCount || leftCount >= roopCount)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 指定された壁との判定
    /// </summary>
    /// <param name="lineObject">判定する壁</param>
    /// <param name="legPoint">足のポイント</param>
    /// <param name="targetPoint">移動先のポイント</param>
    /// <returns>最短距離で壁に阻まれてたらTrue</returns>
    bool IsHitWall(LineObject lineObject, PointObject targetPoint, PointObject legPoint)
    {
        //移動先が線とつながっていない場合は無視する
        if (lineObject == null)
        {
            return false;
        }

        var controlPointVec = (targetPoint.transform.position - property.ControlPoint).normalized;
        var wallVec = lineObject.points[0].transform.position - lineObject.points[1].transform.position;
        var targetPointVec = (legPoint.transform.position - targetPoint.transform.position).normalized;
        var controlPointCross = Vector3.Cross(wallVec, controlPointVec).normalized;
        var targetCross = Vector3.Cross(wallVec, targetPointVec).normalized;

        Debug.Log($"ターゲット{targetPoint.name} 壁{lineObject.name} コントロール{controlPointCross.z} ターゲット{Mathf.Sign(targetCross.z)}");

        //壁のベクトルと移動先ベクトルが同じ場合無視する
        if (Mathf.Approximately(targetCross.z, 0.0f))
        {
            return false;
        }

        //壁のベクトルを挟んで左右にある場合間に壁があると判断する
        if (Mathf.Sign(controlPointCross.z) != Mathf.Sign(targetCross.z))
        {
            return true;
        }

        return false;
    }
}
