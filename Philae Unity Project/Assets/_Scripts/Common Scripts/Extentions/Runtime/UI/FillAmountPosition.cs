using UnityEngine;
using UnityEngine.UI;

namespace hedCommon.extension.runtime
{

    [ExecuteInEditMode]
    public class FillAmountPosition : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _parentRectTransform;
        [SerializeField]
        private Image _imageWithFillAmount;
        [SerializeField]
        private RectTransform _cursor;

        [SerializeField, Range(0, 1)]
        private float minPercentOffset = 0f;
        [SerializeField, Range(0, 1)]
        private float maxPercentOffset = 0f;

        private float _savedLastAmount = -1;

        private void OnEnable()
        {
            if (_imageWithFillAmount == null)
            {
                _imageWithFillAmount = gameObject.GetComponent<Image>();
            }
            if (_parentRectTransform == null)
            {
                _parentRectTransform = gameObject.GetComponent<RectTransform>();
            }
            _savedLastAmount = -1;
            SetupPosition();
        }

        private void Update()
        {
            if (_imageWithFillAmount.fillAmount == _savedLastAmount)
            {
                return;
            }
            SetupPosition();
        }

        public void SetupPosition()
        {
            _savedLastAmount = _imageWithFillAmount.fillAmount;
            ExtRect.SetToFillAmountPositonX(_parentRectTransform, _cursor, _savedLastAmount, minPercentOffset, maxPercentOffset);
        }
    }
}