using hedCommon.geometry.shape3d;
using philae.gravity.attractor.logic;
using philae.gravity.graviton;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.attractor
{
    public class AttractorConeSphereBase : Attractor
    {
        [SerializeField, OnValueChanged("ChangeConeSettings", true)]
        protected ExtConeSphereBase _cone = default;

        public override void InitOnCreation(List<AttractorListerLogic> attractorListerLogic)
        {
            base.InitOnCreation(attractorListerLogic);
            _cone = new ExtConeSphereBase(Position, Rotation, LocalScale, 0.5f, 4f);
        }

        public override Vector3 GetClosestPoint(Graviton graviton, out bool canApplyGravity)
        {            
            Vector3 closestPoint = _cone.GetClosestPoint(graviton.Position);
            Vector3 position = this.GetRightPosWithRange(graviton.Position, closestPoint, _minRangeWithScale / 2, _maxRangeWithScale / 2, out bool outOfRange);
            canApplyGravity = !outOfRange;
            AddOrRemoveGravitonFromList(graviton, canApplyGravity);
            return (position);
        }

        
        public void ChangeConeSettings()
        {
            _cone.MoveSphape(Position, Rotation, LocalScale, _cone.Radius, _cone.Lenght);
        }
        

        public override void Move()
        {
            _cone.MoveSphape(Position, Rotation, LocalScale);
        }

#if UNITY_EDITOR
        protected override void DrawRange(Color color)
        {
            _cone.Draw(color);
            _cone.DrawWithExtraSize(Color.gray, new Vector3(_minRangeWithScale, _minRangeWithScale / 2, _minRangeWithScale));
            _cone.DrawWithExtraSize(color, new Vector3(_maxRangeWithScale, _maxRangeWithScale / 2, _maxRangeWithScale));
        }
#endif
    }
}