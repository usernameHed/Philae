using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace extUnityComponents.transform
{
    

    /// <summary>
    /// this script allow you to lock a child from the parent movements.
    /// ExecuteInEditMode allow us to execute it even in play mode
    /// </summary>
    [ExecuteInEditMode]
    public class LockEveryParentToZero : MonoBehaviour
    {
        [SerializeField] private bool _worksInPlayMode = false;

        /// <summary>
        /// called in play & in editor
        /// </summary>
        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (_worksInPlayMode)
                {
                    LockEverything();
                }
            }
            else
            {
                LockEverything();
            }
#else
            if (_worksInPlayMode)
            {
                LockEverything();
            }
#endif
        }

        private void LockEverything()
        {
            Transform[] parents = transform.GetExtComponentsInParents<Transform>(99, true);
            for (int i = 0; i < parents.Length; i++)
            {
                parents[i].position = Vector3.zero;
            }
        }

    }
}