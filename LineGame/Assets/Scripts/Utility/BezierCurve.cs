using UnityEngine;

namespace Utility {
    public static class BezierCurve
    {
        /// <summary>
        /// 3つのポイントのベジェカーブ
        /// </summary>
        /// <param name="start">開始座標</param>
        /// <param name="end">終端座標</param>
        /// <param name="control">コントロール座標</param>
        /// <param name="t">欲しい座標</param>
        /// <returns></returns>
        public static Vector3 Culc3PointCurve(Vector3 start, Vector3 end, Vector3 control, float t)
        {
            var oneMinusTime = 1.0f - t;
            return oneMinusTime * oneMinusTime * start +
                   2.0f * oneMinusTime * t * control +
                   t * t * end;
        }
    }

}
