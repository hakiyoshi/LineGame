using Field;
using Input;
using Player;
using R3;
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

    [Inject]
    void Construct(PlayerProperty property, PlayerInputAction input, Collider2D collider)
    {
        this.property = property;
        this.input = input;
        this.collider2d = collider;
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

        property.ControlPoint = defaultControlPoint + move;

        //移動判定の当たり判定のオフセットを移動させる
        collider2d.offset = property.CurrentLeftPosition + ((property.CurrentRightPosition - property.CurrentLeftPosition) * 0.5f) + move;
    }

    /// <summary>
    /// 足の現在座標の移動
    /// </summary>
    /// <param name="collider"></param>
    void MoveLeg(Collider2D collider)
    {
        if (collider.TryGetComponent<PointObject>(out var pointObject))
        {
            //自分の足のポイントオブジェクトが同じ場合は無視する
            if (property.LeftPoint.CurrentValue == pointObject || property.RightPoint.CurrentValue == pointObject)
            {
                return;
            }

            //近い足とポイントオブジェクトを入れ替える
            if (Vector3.Distance(property.LeftPoint.CurrentValue.transform.position, pointObject.transform.position) <=
                Vector3.Distance(property.RightPoint.CurrentValue.transform.position, pointObject.transform.position))
            {
                var leftPointObject = property.LeftPoint.CurrentValue;
                property.SetLeftPoint(pointObject);
                property.SetRightPoint(leftPointObject);
            }
            else
            {
                var rightPointObject = property.RightPoint.CurrentValue;
                property.SetRightPoint(pointObject);
                property.SetLeftPoint(rightPointObject);
            }

            //現在の足の座標を変更する
            property.CurrentLeftPosition = property.LeftPointPosition;
            property.CurrentRightPosition = property.RightPointPosition;
        }
    }
}
