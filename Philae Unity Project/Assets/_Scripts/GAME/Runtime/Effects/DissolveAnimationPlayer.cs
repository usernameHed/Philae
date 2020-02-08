using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DissolveAnimationPlayer : MonoBehaviour
{
    [SerializeField] private AnimationCurve _dissolveOverTime = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField, Min(0.00001f)] private float _duration = 1.0f;
    [SerializeField] private UnityEvent _onEndPlay = new UnityEvent();
    [SerializeField] private Renderer _targetRenderer = null;

    #region Currents
    private IEnumerator _dissolveCoroutine = null;
    private int _dissolveAmountID = Shader.PropertyToID("_DissolveAmount");
    #endregion


    //Debug
   // private void OnGUI() {
      // if (GUILayout.Button("Debug Dissolve")) {
       //    Play();
     //  }
   // }

    #region Animation
    /// <summary>
    /// Plays the dissolve animation
    /// </summary>
    public void Play()
    {
        if (_dissolveCoroutine != null)
        {
            StopCoroutine(_dissolveCoroutine);
        }

        _dissolveCoroutine = DissolveCoroutine();
        StartCoroutine(_dissolveCoroutine);
    }

    private IEnumerator DissolveCoroutine()
    {

        for (float f = 0f; f <= 1; f += Time.deltaTime / _duration)
        {
            float amount = _dissolveOverTime.Evaluate(f);
            _targetRenderer.material.SetFloat(_dissolveAmountID, amount);

            yield return null;
        }

        _onEndPlay.Invoke();
        yield return null;
    }
    #endregion
}
