using hedCommon.extension.propertyAttribute.animationCurve;
using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace hedCommon.extension.runtime.animate
{
    [ExecuteInEditMode]
    public class AnimateFillAmount : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private float _fillAmountFrom = 0f;
        [SerializeField]
        private float _fillAmountTo = 1f;
        [SerializeField, Curve(0, 1, 0, 1)]
        private AnimationCurve _curve = new AnimationCurve();
        [SerializeField]
        private FillAmountPosition _fillAmountPositionReference;

        private FrequencyChrono _chrono = new FrequencyChrono();

        public delegate void AnimationEnd();
        public AnimationEnd AnimationEnded;

        [SerializeField]
        private float _time = 1f;

        private bool _animationLaunched = false;

        public void Init(float from, float to, AnimationEnd animationEnd)
        {
            _fillAmountFrom = from;
            _fillAmountTo = to;
            AnimationEnded = animationEnd;
            InitStartValue();
        }

        public void Animate(float from, float to, float time)
        {
            _fillAmountFrom = from;
            _fillAmountTo = to;
            _time = time;
            _animationLaunched = true;
            _chrono.StartChrono(_time, false);
            InitStartValue();
        }

        [ContextMenu("Animate")]
        public void Animate()
        {
            Animate(_fillAmountFrom, _fillAmountTo, _time);
        }

        private void Update()
        {
            if (!_animationLaunched)
            {
                return;
            }

            Calculate();
        }

        private void InitStartValue()
        {
            float percent = 0;
            float percentEased = _curve.Evaluate(percent);
            float remappedValue = ExtMathf.Remap(percentEased, 0, 1, _fillAmountFrom, _fillAmountTo);
            _image.fillAmount = remappedValue;
        }

        private void Calculate()
        {
            if (_chrono.IsRunning())
            {
                float percent = _chrono.GetCurrentPercentFromTheEnd();
                float percentEased = _curve.Evaluate(percent);
                float remappedValue = ExtMathf.Remap(percentEased, 0, 1, _fillAmountFrom, _fillAmountTo);
                _image.fillAmount = remappedValue;
            }
            else
            {
                _image.fillAmount = _fillAmountTo;
                _animationLaunched = false;
                if (AnimationEnded != null)
                {
                    AnimationEnded.Invoke();
                }
            }
            if (_fillAmountPositionReference != null)
            {
                _fillAmountPositionReference.SetupPosition();
            }
        }
    }
}