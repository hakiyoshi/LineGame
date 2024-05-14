using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIeld
{
    /// <summary>
    /// �t�B�[���h��ɐ���\������I�u�W�F�N�g
    /// �ڑ�����Ă���_2���Ǘ����Ă���
    /// </summary>
    public class LineObject : MonoBehaviour
    {
        LineRenderer lineRenderer = null;

        const int MaxPointCount = 2;

        /// <summary>
        /// �����Ȃ��_�I�u�W�F�N�g
        /// </summary>
        [SerializeField] PointObject[] points = new PointObject[MaxPointCount];

        private void Awake()
        {
            if (!TryGetComponent(out lineRenderer))
            {
                Debug.Assert(lineRenderer != null);
            }

            //TODO: �Q�[�����s���Ƀ��C����������Ă��Ȃ������牺�̃R�����g���R�����g�A�E�g����
            //DrawLine();
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