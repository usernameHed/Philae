﻿using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using philae.gravity.attractor.gravityOverride;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
    /// <summary>
    /// 
    ///        1
    ///       / \
    ///      /   \
    ///     /     \
    ///    /       \
    ///   /  _____  \
    ///  / -       - \
    ///  \     2     /
    ///    --_____--
    ///    
    /// </summary>
    [Serializable]
    public struct ExtConeSphereBase
    {
        #region Cylinder Serialized Variables cached
        private Vector3 _position;
        public Vector3 Position { get { return (_position); } }
        private Quaternion _rotation;
        public Quaternion Rotation { get { return (_rotation); } }
        private Vector3 _localScale;
        public Vector3 LocalScale { get { return (_localScale); } }

        [SerializeField]
        private ExtCircle _circleBase;
        public ExtCircle Base { get { return (_circleBase); } }

        [SerializeField]
        private float _radius;
        public float Radius { get { return (_radius); } }
        private float _radiusSquared;
        [SerializeField]
        private float _lenght;
        public float Lenght { get { return (_lenght); } }
        [SerializeField]
        private float _lenghtSquared;
        [SerializeField]
        private float _realRadius;
        [SerializeField]
        private float _realSquaredRadius;
        public float RealRadius { get { return (_realRadius); } }
        [SerializeField]
        private Matrix4x4 _coneMatrix;
        [SerializeField]
        private Vector3 _p1;
        public Vector3 P1 { get { return (_p1); } }
        [SerializeField]
        private Vector3 _p2;
        public Vector3 P2 { get { return (_p2); } }
        [SerializeField]
        private Vector3 _delta;
        [SerializeField]
        private float _deltaDistSquared;
        [SerializeField]
        private float _deltaDist;
        #endregion

        public ExtConeSphereBase(Vector3 position,
            Quaternion rotation,
            Vector3 localScale,
            float radius,
            float lenght) : this()
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _radius = radius;
            _lenght = lenght;
            _lenghtSquared = _lenght * _lenght;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public ExtConeSphereBase(Vector3 p1, Vector3 p2, float radius = 0.25f) : this()
        {
            _position = ExtVector3.GetMeanOfXPoints(p1, p2);
            _rotation = ExtRotation.QuaternionFromLine(p1, p2, Vector3.up);
            _rotation = ExtRotation.RotateQuaternion(_rotation, new Vector3(90, 0, 0));

            _localScale = new Vector3(1, 1, 1);

            _coneMatrix = Matrix4x4.TRS(_position, _rotation, _localScale * 1);

            //why radius a 0.25, and lenght * 0.8 ?? I don't know,
            //it's there to match the first constructor(position, rotation, scale)
            _radius = radius;
            _lenght = ExtVector3.Distance(p2, p1) * 0.8f;

            _lenghtSquared = _lenght * _lenght;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;

            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _coneMatrix = Matrix4x4.TRS(_position, _rotation, _localScale * _radius);
            Vector3 size = new Vector3(0, _lenght / 2, 0);
            _p1 = _coneMatrix.MultiplyPoint3x4(Vector3.zero - ((-size)));
            _p2 = _coneMatrix.MultiplyPoint3x4(Vector3.zero + ((-size)));
            _delta = _p2 - _p1;
            _deltaDist = _delta.magnitude;
            _deltaDistSquared = ExtVector3.DotProduct(_delta, _delta);

            _circleBase.MoveSphape(_p2, _coneMatrix.DownFast(), _realRadius);
        }


        private float MaxXY(Vector3 size)
        {
            return (Mathf.Max(size.x, size.z));
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale, float radius, float lenght)
        {
            _radius = radius;
            _lenght = lenght;
            _lenghtSquared = _lenght * _lenght;
            MoveSphape(position, rotation, localScale);
        }

        public void ChangeRadius(float radius)
        {
            _radius = radius;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public void ChangeLenght(float lenght)
        {
            _lenght = lenght;
            _lenghtSquared = _lenght * _lenght;
            UpdateMatrix();
        }


        /// <summary>
        /// return true if the position is inside the sphape
        /// </summary>
        public bool IsInsideShape(Vector3 k)
        {
            return (false);
        }

        /// <summary>
        /// Return the closest point on the surface of the cone
        ///   
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            float dist = ExtVector3.DotProduct(k - _p1, _delta);

            //k projection is outside the [_p1, _p2] interval, closest to _p1
            if (dist <= 0.0f)
            {
                return (_p1);
            }
            //k projection is outside the [_p1, p2] interval, closest to _p2
            else if (dist >= _deltaDistSquared)
            {
                bool canApplyGravity = _circleBase.GetClosestPointOnDisc(k, out Vector3 closestPoint);
                if (!canApplyGravity)
                {
                    throw new Exception("can't be possible, see AllowBottom for more information");
                }
                return (closestPoint);
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                dist = dist / _deltaDistSquared;
                Vector3 pointOnLine = _p1 + dist * _delta;
                Vector3 closestPoint = ThalesCalculation(k, pointOnLine);
                return (closestPoint);
            }
        }

        public Vector3 GetPointOnCircle()
        {
            return (_circleBase.Point + _coneMatrix.ForwardFast());
        }

        public float GetDistanceFromPoint(Vector3 k)
        {
            return (GetClosestPoint(k).magnitude);
        }

        public bool GetClosestPointIfWeCan(Vector3 k, GravityOverrideConeSphereBase gravityOverride, out Vector3 closestPoint)
        {
            closestPoint = Vector3.zero;

            if (!gravityOverride.CanApplyGravity)
            {
                return (false);
            }

            float dist = ExtVector3.DotProduct(k - _p1, _delta);
            //k projection is outside the [_p1, _p2] interval, closest to _p1
            if (dist <= 0.0f)
            {
                if (!gravityOverride.Top)
                {
                    return (false);
                }
                closestPoint = _p1;
                return (true);
            }
            //k projection is outside the [_p1, p2] interval, closest to _p2
            else if (dist >= _deltaDistSquared)
            {
                return (_circleBase.GetClosestPointOnDiscIfWeCan(k, gravityOverride.Base, out closestPoint));
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                if (!gravityOverride.Trunk)
                {
                    return (false);
                }

                dist = dist / _deltaDistSquared;
                Vector3 pointOnLine = _p1 + dist * _delta;
                closestPoint = ThalesCalculation(k, pointOnLine);
                
                return (true);
            }
        }

        private Vector3 ThalesCalculation(Vector3 k, Vector3 C)
        {
            ///         A (_p1)
            ///        /|\
            ///       / | \
            ///      /  |  \
            ///     /   |   \
            ///    /    |    \
            ///   /     C--?--E--------- K
            ///  /      |      \
            /// /_______B_______D

            float AC = (C - _p1).magnitude;
            float AB = _deltaDist;
            float BD = _realRadius;

            float CE = (AC / AB) * BD;

            return (C + (k - C).FastNormalized() * CE);
        }

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            Debug.DrawLine(_p1, _p2, color);
            ExtDrawGuizmos.DrawLabel(_p1, "1", color);
            _circleBase.Draw(color, false, "2");
            Debug.DrawLine(_p1, _p2 + _coneMatrix.RightFast(), color);
            Debug.DrawLine(_p1, _p2 - _coneMatrix.RightFast(), color);
            Debug.DrawLine(_p1, _p2 + _coneMatrix.ForwardFast(), color);
            Debug.DrawLine(_p1, _p2 - _coneMatrix.ForwardFast(), color);
        }
#endif

        //end class
    }
    //end nameSpace
}