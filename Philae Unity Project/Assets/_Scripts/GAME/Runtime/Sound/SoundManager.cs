using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using hedCommon.singletons;
using System.Linq;

namespace philae.sound
{
    public class SoundManager : SingletonSerializedMono<SoundManager>
    {
        [SerializeField]
        public Dictionary<string, FmodEventEmitter> SoundsEmitter = new Dictionary<string, FmodEventEmitter>();

        public void Init()
        {
            var badKeys = SoundsEmitter.Where(pair => pair.Value == null)
                        .Select(pair => pair.Key)
                        .ToList();
            foreach (var badKey in badKeys)
            {
                SoundsEmitter.Remove(badKey);
            }
        }

        /// <summary>
        /// appelé lorsque la state de la musique a changé
        /// </summary>
        private void StateMusicChanged(string musicName, string stateName, int musicState)
        {
            PlaySound(GetEmitter(musicName), stateName, musicState);
        }

        /// <summary>
        /// ajoute une key dans la liste
        /// </summary>
        public void AddKey(string key, FmodEventEmitter value)
        {
            foreach (KeyValuePair<string, FmodEventEmitter> sound in SoundsEmitter)
            {
                if (key == sound.Key)
                {
                    SoundsEmitter[sound.Key] = value;
                    return;
                }
            }
            SoundsEmitter.Add(key, value);
        }

        /// <summary>
        /// ajoute une key dans la liste
        /// </summary>
        public void DeleteKey(string key, FmodEventEmitter value)
        {
            foreach (KeyValuePair<string, FmodEventEmitter> sound in SoundsEmitter)
            {
                if (key == sound.Key)
                {
                    SoundsEmitter.Remove(key);
                    return;
                }
            }
            //Debug.Log("key sound not found");
        }

        private FmodEventEmitter GetEmitter(string soundTag)
        {
            foreach (KeyValuePair<string, FmodEventEmitter> sound in SoundsEmitter)
            {
                if (soundTag == sound.Key)
                {
                    return (sound.Value);
                }
            }
            return (null);
        }

        /// <summary>
        /// joue un son de menu (sans emmiter)
        /// </summary>
        public void PlaySound(string soundTag, bool play = true)
        {
            if (soundTag == null || soundTag == "")
                return;

            if (!soundTag.Contains("event:/"))
                soundTag = "event:/SFX/" + soundTag;
            PlaySound(GetEmitter(soundTag), play);
            //FMODUnity.RuntimeManager.PlayOneShot("2D sound");   //methode 1 
        }

        /// <summary>
        /// ici play l'emitter (ou le stop)
        /// </summary>
        /// <param name="emitterScript"></param>
        public void PlaySound(FmodEventEmitter emitterScript, bool play = true)
        {
            if (!emitterScript)
            {
                Debug.LogWarning("Emmiter SOund not found !!");
                return;
            }

            if (play)
                emitterScript.Play();
            else
                emitterScript.Stop();
        }

        /// <summary>
        /// ici change le paramettre de l'emitter
        /// </summary>
        /// <param name="emitterScript"></param>
        public void PlaySound(FmodEventEmitter emitterScript, string paramName, float value)
        {
            emitterScript.SetParameterValue(paramName, value);
        }

    }
}