using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIeld
{
    /// <summary>
    /// �t�B�[���h��ɔz�u�����_�I�u�W�F�N�g
    /// �v���C���[�̑���ɂȂ�
    /// </summary>
    public class PointObject : MonoBehaviour
    {
        const int MaxLineObjectCount = 4;

        public const int UpLineObject = 0;
        public const int LeftLineObject = 1;
        public const int DownLineObject = 2;
        public const int RightLineObject = 3;

        /// <summary>
        /// �_�ɐڑ�����Ă�����I�u�W�F�N�g�̎��
        /// ���I�u�W�F�N�g���Ȃ��ꍇ��NULL
        /// 1:��@2:�E�@3:���@4:��
        /// </summary>
        [field: SerializeField]
        public LineObject[] lineObject { get; private set; } = new LineObject[MaxLineObjectCount];
    }
}

