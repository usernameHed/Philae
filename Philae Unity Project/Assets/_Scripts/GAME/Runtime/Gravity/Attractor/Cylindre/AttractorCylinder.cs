using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorCylinder : Attractor
    {
        [SerializeField, OnValueChanged("ChangeCylinderSettings", true)]
        private ExtCylinder _cylinder = default;
        [SerializeField]
        private bool _applyGravityOnTop = true;
        [SerializeField]
        private bool _applyGravityOnBottom = true;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _cylinder = new ExtCylinder(Position, Rotation, LocalScale, 0.5f, 4f);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {
            Vector3 closestPoint = _cylinder.GetClosestPoint(graviton.Position);
            canApplyGravity = true;
            return (closestPoint);


            //Vector3 position = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            //canApplyGravity = !outOfRange;
            //AddOrRemoveGravitonFromList(graviton, canApplyGravity);

            //return (position);
        }

        public void ChangeCylinderSettings()
        {
            _cylinder.MoveSphape(Position, Rotation, LocalScale, _cylinder.Radius, _cylinder.Lenght);
        }

        public override void Move()
        {
            _cylinder.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _cylinder.Draw(color);
            //_cylinder.DrawWithExtraSize(Color.gray, new Vector3(_minRangeWithScale, _minRangeWithScale / 2, _minRangeWithScale));
            //_cylinder.DrawWithExtraSize(color, new Vector3(_maxRangeWithScale, _maxRangeWithScale / 2, _maxRangeWithScale));
        }
#endif
    }
}