using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    public static class ExtAnimation
    {
        public static bool IsAnimationPlaying(this Animator animator)
        {
            return animator.IsInTransition(0) || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
        }
    }
}