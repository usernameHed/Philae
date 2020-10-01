using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace hedCommon.extension.runtime
{
    public static class ExtSounds
    {
        public static double PreciseLenght(this AudioSource audio)
        {
            return (double)audio.clip.samples / audio.clip.frequency;
        }
    }
}