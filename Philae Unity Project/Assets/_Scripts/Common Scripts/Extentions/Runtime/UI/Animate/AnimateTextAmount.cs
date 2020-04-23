using UnityEngine;
using UnityEngine.UI;
using TMPro;
using hedCommon.time;
using hedCommon.extension.runtime.animationCurve;
using hedCommon.extension.propertyAttribute.animationCurve;

namespace hedCommon.extension.runtime.animate
{
    [ExecuteInEditMode]
    public class AnimateTextAmount : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private string _preText = "";
        [SerializeField]
        private string _postText = "";

        [SerializeField]
        private int _fillAmountFrom = 0;
        [SerializeField]
        private int _fillAmountTo = 1;
        [SerializeField, Curve(0, 1, 0, 1)]
        private AnimationCurve _curve = new AnimationCurve();


        private FrequencyChrono _chrono = new FrequencyChrono();

        public delegate void AnimationEnd();
        public AnimationEnd AnimationEnded;

        [SerializeField]
        private float _time = 1f;

        private bool _animationLaunched = false;

        public void Init(int from, int to, AnimationEnd animationEnd, string preText, string postText)
        {
            _fillAmountFrom = from;
            _fillAmountTo = to;
            _preText = preText;
            _postText = postText;
            AnimationEnded = animationEnd;
            InitStartValue();
        }

        public void Animate(int from, int to, float time, string preText, string postText)
        {
            _preText = preText;
            _postText = postText;
            Animate(from, to, time);
        }

        public void Animate(int from, int to, float time)
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
            _text.text = _preText + remappedValue.ToString("0") + _postText;
        }

        private void Calculate()
        {
            if (_chrono.IsRunning())
            {
                float percent = _chrono.GetCurrentPercentFromTheEnd();
                float percentEased = _curve.Evaluate(percent);
                float remappedValue = ExtMathf.Remap(percentEased, 0, 1, _fillAmountFrom, _fillAmountTo);
                _text.text = _preText + remappedValue.ToString("0") + _postText;
            }
            else
            {
                _text.text = _preText + _fillAmountTo.ToString("0") + _postText;
                _animationLaunched = false;
                if (AnimationEnded != null)
                {
                    AnimationEnded.Invoke();
                }
            }
        }
    }
}