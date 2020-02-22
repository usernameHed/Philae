using hedCommon.extension.runtime;
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
        private float _deltaSquared;
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
            _deltaSquared = ExtVector3.DotProduct(_delta, _delta);

            _circleBase.MoveSphape(_p2, _coneMatrix.Down(), _realRadius);
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
            else if (dist >= _deltaSquared)
            {
                return (_circleBase.GetClosestPointOnDisc(k, out bool canApplyGravity));
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                //here do calculation
                return (_p1 - _p2);
            }
        }

        public float GetDistanceFromPoint(Vector3 k)
        {
            return (GetClosestPoint(k).magnitude);
        }

        public Vector3 GetClosestPointIfWeCan(Vector3 k, out bool canApplyGravity, GravityOverrideCone gravityOverride)
        {
            if (!gravityOverride.CanApplyGravity)
            {
                canApplyGravity = false;
                return (k);
            }

            canApplyGravity = true;
            float dist = ExtVector3.DotProduct(k - _p1, _delta);
            //k projection is outside the [_p1, _p2] interval, closest to _p1
            if (dist <= 0.0f)
            {
                canApplyGravity = gravityOverride.Top;
                return (canApplyGravity ? _p1 : Vector3.zero);
            }
            //k projection is outside the [_p1, p2] interval, closest to _p2
            else if (dist >= _deltaSquared)
            {
                return (_circleBase.GetClosestPointOnDiscIfWeCan(k, out canApplyGravity, gravityOverride.Base));
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                if (!gravityOverride.Trunk)
                {
                    canApplyGravity = false;
                    return (k);
                }

                //here do calculation
                return (_p1 - _p2);
            }
        }

#if UNITY_EDITOR
        public void Draw(Color color)
        {
            Debug.DrawLine(_p1, _p2, color);
            ExtDrawGuizmos.DrawLabel(_p1, "1", color);
            _circleBase.Draw(color, false, "2");
        }

        public void DrawWithExtraSize(Color color, Vector3 extraSize)
        {
            if (extraSize.Maximum() <= 1f)
            {
                return;
            }

            Matrix4x4 coneMatrix = Matrix4x4.TRS(_position, _rotation, (_localScale + extraSize) * _radius);
            Vector3 size = new Vector3(0, _lenght / 2, 0);
            Vector3 p1 = coneMatrix.MultiplyPoint3x4(Vector3.zero + ((-size)));
            Vector3 p2 = coneMatrix.MultiplyPoint3x4(Vector3.zero - ((-size)));
            float realRadius = _radius * MaxXY(_localScale + extraSize);

            Debug.DrawLine(p1, p2, color);
            ExtDrawGuizmos.DrawLabel(p1, "1", color);
            _circleBase.DrawWithExtraSize(color, realRadius, false, "2");
        }
#endif

        //end class
    }
    //end nameSpace
}