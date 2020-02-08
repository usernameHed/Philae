using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
    public class ExtCylindre
    {
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _localScale;
        [SerializeField, ReadOnly]
        protected float _radius;
        public float Radius { get { return (_radius); } }
        private float _radiusSquared;
        [SerializeField, ReadOnly]
        private float _lenght;
        public float Lenght { get { return (_lenght); } }
        private float _lenghtSquared;

        protected float _realRadius;
        private float _realSquaredRadius;

        private Matrix4x4 _cubeMatrix;
        protected Vector3 _p1;
        protected Vector3 _p2;
        private Vector3 _delta;
        private float _deltaSquared;

        public ExtCylindre(Vector3 position,
            Quaternion rotation,
            Vector3 localScale,
            float radius,
            float lenght)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _radius = radius;
            _lenght = lenght;
            _lenghtSquared = _lenght * _lenght;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * _localScale.Maximum();
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _cubeMatrix = Matrix4x4.TRS(_position, _rotation, _localScale * _radius);
            Vector3 size = new Vector3(0, _lenght / 2, 0);
            _p1 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero + ((-size)));
            _p2 = _cubeMatrix.MultiplyPoint3x4(Vector3.zero - ((-size)));
            _delta = _p2 - _p1;
            _deltaSquared = _delta.sqrMagnitude;
        }

        public virtual void Draw(Color color)
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DrawCylinder(_p1, _p2, color, _realRadius);
#endif
        }

        public virtual void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _realRadius = _radius * _localScale.Maximum();
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public virtual void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale, float radius, float lenght)
        {
            _radius = radius;
            _lenght = lenght;
            _lenghtSquared = _lenght * _lenght;
            MoveSphape(position, rotation, localScale);
        }

        public virtual void ChangeRadius(float radius)
        {
            _radius = radius;
            _radiusSquared = _radius * _radius;
            _realRadius = _radius * _localScale.Maximum();
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        public virtual void ChangeLenght(float lenght)
        {
            _lenght = lenght;
            _lenghtSquared = _lenght * _lenght;
            UpdateMatrix();
        }


        /// <summary>
        /// return true if the position is inside the sphape
        /// </summary>
        public virtual bool IsInsideShape(Vector3 pointToTest)
        {
#if UNITY_EDITOR
            if (_p1 == _p2 || _radius == 0)
            {
                return (false);
            }
#endif

            Vector3 pDir = pointToTest - _p1;
            float dot = Vector3.Dot(_delta, pDir);

            if (dot < 0f || dot > _deltaSquared)
            {
                return (false);
            }

            float dsq = pDir.x * pDir.x + pDir.y * pDir.y + pDir.z * pDir.z - dot * dot / _deltaSquared;

            if (dsq > _realSquaredRadius)
            {
                return (false);
            }
            else
            {
                return (true);
            }
        }

        //end class
    }
    //end nameSpace
}