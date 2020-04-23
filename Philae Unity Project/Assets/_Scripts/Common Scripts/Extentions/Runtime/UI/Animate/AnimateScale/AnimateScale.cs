using hedCommon.extension.propertyAttribute.animationCurve;
using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime.animate.scale
{
    [ExecuteInEditMode]
    public class AnimateScale : MonoBehaviour
    {
        [SerializeField]
        private float _timeAnimation = 0.5f;
        [SerializeField]
        private Vector3 _maxScale = new Vector3(2, 2, 2);
        [SerializeField, Curve(0, 1, 0, 1)]
        private AnimationCurve _ease = ExtAnimationCurve.GetDefaultCurve(ExtAnimationCurve.EaseType.EASE_IN_OUT);

        [SerializeField]
        private Transform _toAnimate;
        private FrequencyChrono _chrono = new FrequencyChrono();
        private Vector3 _initialScale;

        private void Awake()
        {
            this.enabled = false;
        }

        public void Animate()
        {
            if (_chrono.IsRunning())
            {
                ResetAnimation();
            }

            _initialScale = _toAnimate.localScale;
            this.enabled = true;
            _chrono.StartChrono(_timeAnimation, false);
        }

        public void ResetAnimation()
        {
            _toAnimate.localScale = _initialScale;
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
            Scale();
        }

        private void Scale()
        {
            _toAnimate.localScale = Vector3.Lerp(_initialScale, _maxScale, _ease.Evaluate(_chrono.GetCurrentPercentFromTheEnd()));
        }

        private void AnimateEnd()
        {
            _toAnimate.localScale = _initialScale;
            this.enabled = false;
        }
    }
}