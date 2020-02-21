using hedCommon.extension.runtime;
using hedCommon.geometry.shape2d;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.geometry.shape3d
{
    /// <summary>
    /// Capsule 
    ///       _3_
    ///    /       \
    ///   /  -----  \
    ///  |/         \|
    ///  |\    1    /|
    ///  | --_____-- |
    ///  |           |
    ///  |           |
    ///  |           |
    ///  |           |
    ///  |           |
    ///  |           |
    ///  \     2     /
    ///  | --_____-- |
    ///   \         /
    ///     - _4_ -
    ///    
    /// </summary>
    [Serializable]
    public class ExtCapsule : ExtCylinder
    {
        public bool SphereBottom = true;
        public bool SphereTop = true;

        private ExtSphere _topSphere;
        private ExtSphere _bottomSphere;

        public ExtCapsule(Vector3 position,
            Quaternion rotation,
            Vector3 localScale,
            float radius,
            float lenght,
            bool top = true,
            bool bottom = true)
            : base(position, rotation, localScale, radius, lenght)
        {
            SphereTop = top;
            SphereBottom = bottom;
            _topSphere = new ExtSphere(_p1, _realRadius);
            _bottomSphere = new ExtSphere(_p2, _realRadius);
        }

        public ExtCapsule(ExtCylinder cylinder, bool top = true, bool bottom = true)
            : base(cylinder.Position, cylinder.Rotation, cylinder.LocalScale, cylinder.Radius, cylinder.Lenght)
        {
            SphereTop = top;
            SphereBottom = bottom;
            _topSphere = new ExtSphere(_p1, _realRadius);
            _bottomSphere = new ExtSphere(_p2, _realRadius);
        }

        public override void Draw(Color color)
        {
#if UNITY_EDITOR
            ExtDrawGuizmos.DebugCapsuleFromInsidePoint(_p1, _p2, color, _realRadius, 0, true, true, SphereTop, SphereBottom);
#endif
        }

        public override void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            base.MoveSphape(position, rotation, localScale);
            _topSphere.MoveSphape(_p1, _realRadius);
            _bottomSphere.MoveSphape(_p2, _realRadius);
        }

        public override void MoveSphape(Vector3 position, Quaternion rotation, Vector3 localScale, float radius, float lenght)
        {
            base.MoveSphape(position, rotation, localScale, radius, lenght);
            _topSphere.MoveSphape(_p1, _realRadius);
            _bottomSphere.MoveSphape(_p2, _realRadius);
        }

        public override void ChangeRadius(float radius)
        {
            base.ChangeRadius(radius);
            _topSphere.MoveSphape(_p1, _realRadius);
            _bottomSphere.MoveSphape(_p2, _realRadius);
        }

        public override void ChangeLenght(float lenght)
        {
            base.ChangeLenght(lenght);
            _topSphere.MoveSphape(_p1, _realRadius);
            _bottomSphere.MoveSphape(_p2, _realRadius);
        }

        /// <summary>
        /// return true if the position is inside the sphape
        /// </summary>
        public override bool IsInsideShape(Vector3 pointToTest)
        {
            bool isInsideTopSphere = SphereBottom && _topSphere.IsInsideShape(pointToTest);
            if (isInsideTopSphere)
            {
                return (true);
            }
            bool isInsideBottomSphere = SphereTop && _bottomSphere.IsInsideShape(pointToTest);
            if (isInsideBottomSphere)
            {
                return (true);
            }
            return (base.IsInsideShape(pointToTest));
        }

        //end class
    }
    //end nameSpace
}