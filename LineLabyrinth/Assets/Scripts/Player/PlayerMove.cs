using Input;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class PlayerMove : MonoBehaviour
{
    PlayerProperty property;
    PlayerInputAction input;

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
    void Construct(PlayerProperty property, PlayerInputAction input)
    {
        this.property = property;
        this.input = input;
    }

    private void Update()
    {
        MoveControlPoint();
    }

    /// <summary>
    /// コントロールポイント制御
    /// </summary>
    void MoveControlPoint()
    {
        var moveInput = input.Player.Move.ReadValue<Vector2>();

        var defaultControlPoint = property.CulcDefaultControlPoint();

        property.ControlPoint = defaultControlPoint + new Vector3(moveInput.x, moveInput.y, 0.0f) * moveDistance;
    }
}
