using UnityEngine;
using System.Collections;

namespace philae.sound
{
    [ExecuteInEditMode]
    public class FmodEventEmitter : MonoBehaviour
    {
        public string additionnalName = "";
        public Transform addIdOfObject;

        [SerializeField]
        private FMODUnity.StudioEventEmitter _emitter;   //l'emitter attaché à l'objet


        private void OnEnable()
        {
            if (_emitter == null)
            {
                _emitter = gameObject.GetComponent<FMODUnity.StudioEventEmitter>();  //init l'emitter
            }
            string addParent = (addIdOfObject) ? addIdOfObject.GetInstanceID().ToString() : "";
            if (_emitter && _emitter.Event != "" && SoundManager.Instance != null)
            {
                //Debug.Log("soundManager: " + SoundManager.Instance);
                SoundManager.Instance.AddKey(_emitter.Event + additionnalName + addParent, this);
            }
        }

        /// <summary>
        /// play l'emmiter
        /// </summary>
        public void Play()
        {
            if (!gameObject || !_emitter)
            {
                return;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif

            _emitter.Play();
        }

        /// <summary>
        /// stop l'emmiter
        /// </summary>
        public void Stop()
        {
            if (!gameObject || !_emitter)
                return;
            _emitter.Stop();
        }

        /// <summary>
        /// change les paramettres de l'emmiter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        public void SetParameterValue(string paramName, float value)
        {
            if (!gameObject || !_emitter)
                return;
            _emitter.SetParameter(paramName, value);
        }

        private void OnDisable()
        {
            string addParent = (addIdOfObject) ? addIdOfObject.GetInstanceID().ToString() : "";
            if (_emitter && _emitter.Event != "" && SoundManager.Instance)
            {
                SoundManager.Instance.DeleteKey(_emitter.Event + additionnalName + addParent, this);
            }
        }
    }
}