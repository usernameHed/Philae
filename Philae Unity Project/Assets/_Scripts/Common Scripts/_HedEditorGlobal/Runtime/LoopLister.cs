using hedCommon.extension.runtime;
using hedCommon.singletons;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace hedCommon.editorGlobal
{
    public class LoopLister : SingletonMono<LoopLister>
    {
        public List<IUpdater> ToUpdate = new List<IUpdater>();

        public void Init()
        {
            ToUpdate.Clear();
            IUpdater[] updaters = ExtFind.GetScripts<IUpdater>();
            ToUpdate = updaters.ToList();
        }

        public void AddIfNoExist(IUpdater updater)
        {
            bool added = ToUpdate.AddIfNotContain(updater);
            if (added)
            {
                LoopUpdater.Instance.AddUpdater(updater);
            }
        }

        public void RemoveIfExist(IUpdater updater)
        {
            bool removed = ToUpdate.Remove(updater);
            if (removed)
            {
                LoopUpdater.Instance.RemoveUpdater(updater);
            }
        }
    }
}