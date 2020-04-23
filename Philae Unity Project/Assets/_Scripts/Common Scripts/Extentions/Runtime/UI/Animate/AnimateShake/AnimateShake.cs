using hedCommon.extension.propertyAttribute.animationCurve;
using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime.animate.shake
{
    [ExecuteInEditMode]
    public class AnimateShake : MonoBehaviour
    {
        [SerializeField]
        private float _timeAnimation = 0.5f;
        [SerializeField]
        private Vector3 _maxOffset = new Vector3(2f, 2f, 0);
        [SerializeField, Curve(0, 1, 0, 1)]
        private AnimationCurve _ease = ExtAnimationCurve.GetDefaultCurve(ExtAnimationCurve.EaseType.EASE_IN_OUT);

        [SerializeField]
        private Transform _toAnimate;
        private FrequencyChrono _chrono = new FrequencyChrono();
        private Vector3 _initialLocalPosition;

        private void Awake()
        {
            this.enabled = false;
        }

        public void Animate()
        {
            if (_toAnimate == null)
            {
                return;
            }

            if (_chrono.IsRunning())
            {
                ResetAnimation();
            }

            this.enabled = true;
            _initialLocalPosition = _toAnimate.localPosition;
            _chrono.StartChrono(_timeAnimation, false);
        }

        public void ResetAnimation()
        {
            _toAnimate.localPosition = _initialLocalPosition;
            _chrono.Reset();
            this.enabled = false;
        }

        private void Update()
        {
            if (_chrono.IsFinished())
            {
                AnimateEnd();
                return;
            }
            Shake();
        }

        private void Shake()
        {
            _toAnimate.localPosition = _initialLocalPosition + ExtRandom.GetRandomInsideUnitSphere(GetMaxOffsetEased());
        }

        private Vector3 GetMaxOffsetEased()
        {
            //return (_maxOffset);
            //Vector3 maxOffset = _maxOffset;
            //maxOffset.x = Vector3.Lerp(_initialLocalPosition, _initialLocalPosition + _maxOffset, _ease.Evaluate(_chrono.GetCurrentPercentFromTheEnd()));

            return (Vector3.Lerp(Vector3.zero, _maxOffset, _ease.Evaluate(_chrono.GetCurrentPercentFromTheEnd())));
        }

        private void AnimateEnd()
        {
            _toAnimate.localPosition = _initialLocalPosition;
            this.enabled = false;
        }
    }
}