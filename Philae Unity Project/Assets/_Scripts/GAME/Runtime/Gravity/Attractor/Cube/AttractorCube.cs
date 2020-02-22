using hedCommon.extension.runtime;
using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorCube : Attractor
    {
        [SerializeField, ReadOnly]
        protected ExtCube _cube = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            Debug.Log("here init always ?");
            _cube = new ExtCube(Position, Rotation, LocalScale);
        }

        public override bool GetClosestPoint(Graviton graviton, out Vector3 closestPoint)
        {
            closestPoint = _cube.GetClosestPoint(graviton.Position);
            closestPoint = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);

            bool canApplyGravity = true;
            if (outOfRange)
            {
                canApplyGravity = false;
            }
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (canApplyGravity);
        }

        public override void Move()
        {
            _cube.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _cube.Draw(color, true, true);
            _cube.DrawWithExtraSize(Color.gray, new Vector3(_minRangeWithScale, _minRangeWithScale, _minRangeWithScale));
            _cube.DrawWithExtraSize(color, new Vector3(_maxRangeWithScale, _maxRangeWithScale, _maxRangeWithScale));
        }
#endif
    }
}