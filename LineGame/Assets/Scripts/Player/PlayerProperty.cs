using FIeld;
using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// �v���C���[�̏���ێ�����N���X
    /// VContainer����A�N�Z�X����
    /// </summary>
    public class PlayerProperty
    {
        private ReactiveProperty<PointObject> leftPoint = new();
        public ReadOnlyReactiveProperty<PointObject> LeftPoint => leftPoint;

        private ReactiveProperty<PointObject> rightPoint = new();
        public ReadOnlyReactiveProperty<PointObject> RightPint => rightPoint;

        /// <summary>
        /// �����ɓ_�I�u�W�F�N�g���Z�b�g����
        /// </summary>
        /// <param name="point"></param>
        void SetLeftPoint(PointObject point)
        {
            leftPoint.Value = point;
        }

        /// <summary>
        /// �E���ɓ_�I�u�W�F�N�g���Z�b�g����
        /// </summary>
        /// <param name="point"></param>
        void SetRightPint(PointObject point)
        {
            rightPoint.Value = point;
        }
    }
}

