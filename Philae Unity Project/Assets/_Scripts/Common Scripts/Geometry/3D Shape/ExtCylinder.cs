using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
    [Serializable]
    public class ExtCylinder
    {
        private Vector3 _position;
        public Vector3 Position { get { return (_position); } }
        private Quaternion _rotation;
        public Quaternion Rotation { get { return (_rotation); } }
        private Vector3 _localScale;

        [SerializeField]
        private ExtCircle _circle1;
        [SerializeField]
        private ExtCircle _circle2;

        [SerializeField]
        protected float _radius;
        public float Radius { get { return (_radius); } }
        private float _radiusSquared;
        [SerializeField]
        private float _lenght;
        public float Lenght { get { return (_lenght); } }
        private float _lenghtSquared;

        protected float _realRadius;
        private float _realSquaredRadius;
        public float RealRadius { get { return (_realRadius); } }

        private Matrix4x4 _cylinderMatrix;
        protected Vector3 _p1;
        protected Vector3 _p2;
        private Vector3 _delta;
        private Vector3 _deltaNormalized;
        private float _deltaSquared;

        public ExtCylinder(Vector3 position,
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
            _realRadius = _radius * MaxXY(_localScale);
            _realSquaredRadius = _realRadius * _realRadius;
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            _cylinderMatrix = Matrix4x4.TRS(_position, _rotation, _localScale * _radius);
            Vector3 size = new Vector3(0, _lenght / 2, 0);
            _p1 = _cylinderMatrix.MultiplyPoint3x4(Vector3.zero + ((-size)));
            _p2 = _cylinderMatrix.MultiplyPoint3x4(Vector3.zero - ((-size)));
            _delta = _p2 - _p1;
            _deltaNormalized = _delta.FastNormalized();
            _deltaSquared = ExtVector3.DotProduct(_delta, _delta);

            _circle1.MoveSphape(_p1, -_cylinderMatrix.Up(), _realRadius);
            _circle2.MoveSphape(_p2, _cylinderMatrix.Up(), _realRadius);
        }

        public virtual void Draw(Color color)
        {
#if UNITY_EDITOR
            Debug.DrawLine(_p1, _p2, color);
            _circle1.Draw(color, false, "1");
            _circle2.Draw(color, false, "2");
#endif
        }

        public void DrawWithExtraSize(Color color, Vector3 extraSize)
        {
#if UNITY_EDITOR
            if (extraSize.Maximum() <= 1f)
            {
                return;
            }

            Matrix4x4 cylinderMatrix = Matrix4x4.TRS(_position, _rotation, (_localScale + extraSize) * _radius);
            Vector3 size = new Vector3(0, _lenght / 2, 0);
            Vector3 p1 = cylinderMatrix.MultiplyPoint3x4(Vector3.zero + ((-size)));
            Vector3 p2 = cylinderMatrix.MultiplyPoint3x4(Vector3.zero - ((-size)));
            float realRadius = _radius * MaxXY(_localScale + extraSize);
            ExtDrawGuizmos.DrawCylinder(p1, p2, color, realRadius);

            /*
            ExtCircle circle1 = new ExtCircle(p1, -cylinderMatrix.Up(), realRadius);
            ExtCircle circle2 = new ExtCircle(p2, cylinderMatrix.Up(), realRadius);
            circle1.Draw(color, false, "1");
            circle2.Draw(color, false, "2");
            */
#endif
        }

        private float MaxXY(Vector3 size)
        {
            return (Mathf.Max(size.x, size.z));
        }

        public virtual void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            _position = position;
            _rotation = rotation;
            _localScale = localScale;
            _realRadius = _radius * MaxXY(_localScale);
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
            _realRadius = _radius * MaxXY(_localScale);
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
        public virtual bool IsInsideShape(Vector3 k)
        {
#if UNITY_EDITOR
            if (_p1 == _p2 || _radius == 0)
            {
                return (false);
            }
#endif

            Vector3 pDir = k - _p1;
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

        /// <summary>
        /// Return the closest point on the surface of the cylinder
        /// https://diego.assencio.com/?index=ec3d5dfdfc0b6a0d147a656f0af332bd
        ///   
        /// </summary>
        public Vector3 GetClosestPoint(Vector3 k)
        {
            float dist = ExtVector3.DotProduct(k - _p1, _delta);

            //k projection is outside the [_p1, _p2] interval, closest to _p1
            if (dist <= 0.0f)
            {
                return (_circle1.GetClosestPointOnDisc(k));
            }
            //k projection is outside the [_p1, p2] interval, closest to _p2
            else if (dist >= _deltaSquared)
            {
                return (_circle2.GetClosestPointOnDisc(k));
            }
            //k projection is inside the [_p1, p2] interval
            else
            {
                dist = dist / _deltaSquared;
                Vector3 pointOnLine = _p1 + dist * _delta;
                Vector3 pointOnSurfaceLine = pointOnLine + ((k - pointOnLine).FastNormalized() * _realRadius);
                return (pointOnSurfaceLine);
            }
        }

        //end class
    }
    //end nameSpace
}