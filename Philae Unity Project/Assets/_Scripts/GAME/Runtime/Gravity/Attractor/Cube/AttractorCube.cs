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
        private ExtCube _cube = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            Debug.Log("here init always ?");
            _cube = new ExtCube(Position, Rotation, LocalScale);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 closestPoint = _cube.GetClosestPoint(graviton.Position);

            Vector3 position = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            canApplyGravity = !outOfRange;
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);

            return (position);
        }

        public override void Move()
        {
            _cube.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _cube.Draw(color);
            _cube.DrawWithExtraSize(Color.gray, new Vector3(_minRangeWithScale, _minRangeWithScale, _minRangeWithScale));
            _cube.DrawWithExtraSize(color, new Vector3(_maxRangeWithScale, _maxRangeWithScale, _maxRangeWithScale));
        }
#endif
    }
}