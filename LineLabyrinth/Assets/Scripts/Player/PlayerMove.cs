using DG.Tweening;
using Field;
using Input;
using UnityEngine;
using VContainer;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        PlayerProperty property;
        PlayerInputAction input;
        Collider2D collider2d;

        /// <summary>
        /// 移動幅
        /// </summary>
        [SerializeField]
        float moveDistance = 1.0f;

        [SerializeField]
        float controlPointMoveSpeed = 0.2f;

        [SerializeField]
        float moveLegSeconds = 0.2f;

        Tweener leftLegMove = null;
        Tweener rightLegMove = null;

        Tweener controlPointMove = null;
        Tweener colliderMove = null;

        [Inject]
        void Construct(PlayerProperty property, PlayerInputAction input, Collider2D collider2d)
        {
            this.property = property;
            this.input = input;
            this.collider2d = collider2d;
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
            controlPointMove = DOVirtual.Vector3(property.ControlPoint, defaultControlPoint + move, controlPointMoveSpeed, x => property.ControlPoint = x);

            //移動判定の当たり判定のオフセットを移動させる
            colliderMove = DOVirtual.Vector3(collider2d.offset, CulcOffsetDefaultPosition() + move, controlPointMoveSpeed, x => collider2d.offset = x);
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

                var setLeftPoint = property.LeftPoint.CurrentValue;
                var setRightPoint = property.RightPoint.CurrentValue;
                PointObject pivotLegPoint = null;

                //近い足とポイントオブジェクトを入れ替える
                if (Vector3.Distance(property.LeftPoint.CurrentValue.transform.position, targetPoint.transform.position) <=
                    Vector3.Distance(property.RightPoint.CurrentValue.transform.position, targetPoint.transform.position))
                {
                    if (IsHitWall(targetPoint, property.LeftPoint.CurrentValue, property.LeftPoint.CurrentValue, property.RightPoint.CurrentValue))
                    {
                        return;
                    }

                    setRightPoint = property.LeftPoint.CurrentValue;
                    setLeftPoint = targetPoint;
                    pivotLegPoint = setRightPoint;
                }
                else
                {
                    if (IsHitWall(targetPoint, property.RightPoint.CurrentValue, property.LeftPoint.CurrentValue, property.RightPoint.CurrentValue))
                    {
                        return;
                    }

                    setLeftPoint = property.RightPoint.CurrentValue;
                    setRightPoint = targetPoint;
                    pivotLegPoint = setLeftPoint;
                }

                //移動先が壁に阻まれていた場合
                for(var i = 0; i < pivotLegPoint.LineObject.Length; i++)
                {
                    var lineObject = pivotLegPoint.LineObject[i];
                    if (pivotLegPoint.LineObject[i] == null)
                    {
                        continue;
                    }

                    var setLegControlPointVec = property.CulcDefaultControlPoint(setLeftPoint, setRightPoint) - pivotLegPoint.transform.position;
                    var controlPointVec = property.CulcDefaultControlPoint() - pivotLegPoint.transform.position;
                    var wallVec = lineObject.CulcWallVec(pivotLegPoint);

                    var setControlPointDot = Vector3.Dot(wallVec, setLegControlPointVec);
                    var controlPointDot = Vector3.Dot(wallVec, controlPointVec);

                    var setControlPointSign = Mathf.Sign(setControlPointDot);
                    var controlPointSign = Mathf.Sign(controlPointDot);

                    //移動後の位置に壁で阻まれている場合左右の足を入れ替える
                    if (IsHitWall(wallVec, controlPointVec, setLegControlPointVec))
                    {
                        setLegControlPointVec = property.CulcDefaultControlPoint(setRightPoint, setLeftPoint) - pivotLegPoint.transform.position;
                        
                        var swapFlag = false;
                        if (controlPointSign > 0.0f && setControlPointSign > 0.0f)
                        {
                            Debug.Log(lineObject.name + " Hit1");
                            swapFlag = true;
                        }
                        else if (pivotLegPoint.LineObject[(i + (pivotLegPoint.LineObject.Length / 2)) % pivotLegPoint.LineObject.Length] != null)
                        {
                            Debug.Log(lineObject.name + " Hit2");
                            swapFlag = true;
                        }
                        else if (pivotLegPoint.LineObject[(i + (pivotLegPoint.LineObject.Length / 2)) % pivotLegPoint.LineObject.Length] == null && 
                                  controlPointSign < 0.0f && CulcTwoSides(wallVec, setLegControlPointVec) < 0.0f)
                        {
                            Debug.Log(lineObject.name + " Hit3");
                            swapFlag = true;
                        }

                        if (swapFlag)
                        {
                            (setRightPoint, setLeftPoint) = (setLeftPoint, setRightPoint);
                        }
                    }
                }

                property.SetRightPoint(setRightPoint);
                property.SetLeftPoint(setLeftPoint);

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
        /// <param name="leftPoint">左足のポイント</param>
        /// <param name="rightPoint">右足のポイント</param>
        /// <returns>壁に阻まれてたらTrue</returns>
        bool IsHitWall(PointObject targetPoint, PointObject legPoint, PointObject leftPoint, PointObject rightPoint)
        {
            for(int i = 0; i < legPoint.LineObject.Length; i++)
            {
                if (IsHitWall(legPoint.LineObject[i], targetPoint, legPoint))
                {
                    var wallVec = legPoint.LineObject[i].CulcWallVec(legPoint).normalized;
                    var targetControlVec = property.CulcDefaultControlPoint(targetPoint, legPoint == leftPoint ? leftPoint : rightPoint) - legPoint.transform.position;
                    var controlPointVec = property.CulcDefaultControlPoint(leftPoint, rightPoint) - legPoint.transform.position;
                    
                    var targetControlTwoSides = CulcTwoSides(wallVec, targetControlVec);
                    var controlPointTwoSides = CulcTwoSides(wallVec, controlPointVec);

                    var controlPointCross = Vector3.Cross(wallVec, controlPointVec.normalized);
                    
                    //連続して隣接している壁が2か所空いていた場合壁に阻まれていない判定にする
                    int rightCount = 0;
                    int leftCount = 0;
                    int roopCount = legPoint.LineObject.Length / 2;
                    for (int j = 1; j < roopCount + 1; j++)
                    {
                        var rightSideLine = legPoint.LineObject[((i - j) + legPoint.LineObject.Length) % legPoint.LineObject.Length];
                        var leftSideLine = legPoint.LineObject[(i + j) % legPoint.LineObject.Length];

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
                    if ((controlPointTwoSides < 0.0f && targetControlTwoSides < 0.0f) ||//移動先が同じ向きでそのまま移動する場合
                        (controlPointCross.z >= 0.0f && rightCount >= roopCount) || //右周りで移動出来る場合
                        (controlPointCross.z < 0.0f && leftCount >= roopCount))//左回りで移動できる場合
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

            var controlPointVec = (property.ControlPoint - targetPoint.transform.position).normalized;
            var wallVec = lineObject.CulcWallVec(legPoint).normalized;
            var targetPointVec = (targetPoint.transform.position - legPoint.transform.position).normalized;

            Debug.Log($"足{legPoint.name} 壁{lineObject.name} コントロール{CulcTwoSides(wallVec, controlPointVec)} ターゲット{CulcTwoSides(wallVec, targetPointVec)}");

            return IsHitWall(wallVec, controlPointVec, targetPointVec);
        }

        bool IsHitWall(Vector3 wallVec, Vector3 playerVec, Vector3 targetVec)
        {
            var wallVecNormal = wallVec.normalized;
            var playerCross = Vector3.Cross(wallVecNormal, playerVec.normalized).normalized;
            var targetCross = Vector3.Cross(wallVecNormal, targetVec).normalized;

            //壁のベクトルと移動先ベクトルが同じ場合無視する
            if (Mathf.Approximately(targetCross.z, 0.0f))
            {
                return false;
            }

            //壁のベクトルを挟んで左右にある場合間に壁があると判断する
            if (!Mathf.Approximately(Mathf.Sign(playerCross.z), Mathf.Sign(targetCross.z)))
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// ターゲットが壁ベクトルより表にいる場合+1　裏にいる場合-1　同じ場合0
        /// </summary>
        /// <param name="wallVec"></param>
        /// <param name="targetVec"></param>
        /// <returns>表にいる場合+1　裏にいる場合-1　同じ場合0</returns>
        float CulcTwoSides(Vector3 wallVec, Vector3 targetVec)
        {
            return Mathf.Sign(Vector3.Dot(wallVec, targetVec));
        }
    }
}
