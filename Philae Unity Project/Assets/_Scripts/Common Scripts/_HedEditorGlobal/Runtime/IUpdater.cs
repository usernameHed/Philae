using hedCommon.extension.runtime;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace hedCommon.editorGlobal
{
    [ExecuteInEditMode]
    public abstract class IUpdater : MonoBehaviour
    {
        public abstract void CustomLoop();
        [FoldoutGroup("IUpdater")]
        public bool CanPlayInEditor = true;
        [FoldoutGroup("IUpdater")]
        public bool PlayInFixedUpdate = true;
        [FoldoutGroup("IUpdater")]
        public int Order = 0;

        private void OnEnable()
        {
            if (LoopLister.Instance)
            {
                LoopLister.Instance.AddIfNoExist(this);
            }
        }

        private void OnDestroy()
        {
            if (LoopLister.Instance)
            {
                LoopLister.Instance.RemoveIfExist(this);
            }
        }
    }
}