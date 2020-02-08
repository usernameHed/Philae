using hedCommon.extension.runtime;
using hedCommon.singletons;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace hedCommon.editorGlobal
{
    /// <summary>
    /// This method is used to call every script which need to be called both in editor, and in runtime,
    /// with a nice deltaTime.
    /// 
    /// In Editor: FixeUpdate is not called, so we have to call Update().
    /// </summary>
    [ExecuteInEditMode]
    public class LoopUpdater : SingletonMono<LoopUpdater>
    {
        public bool ExecuteEverythingInEditor = true;

        public LoopLister[] LoopListers = new LoopLister[0];
        [OnValueChanged("SetOrdersFromListFixedUpdate"), ListDrawerSettings(ListElementLabelName = "Order")]
        public List<IUpdater> IUpdatersFixedUpdate = new List<IUpdater>();
        [OnValueChanged("SetOrdersFromListUpdate"), ListDrawerSettings(ListElementLabelName = "Order")]
        public List<IUpdater> IUpdatersUpdate = new List<IUpdater>();

        public delegate void MethodToCallWhenLooping(IUpdater updater);

        [Button]
        public void Init()
        {
            Init(LoopListers);
        }

        public void Init(LoopLister[] fromArray, params LoopLister[] loopLister)
        {
            LoopListers = ExtArray.Append(fromArray, loopLister);
            IUpdatersFixedUpdate.Clear();
            IUpdatersUpdate.Clear();
            for (int i = 0; i < LoopListers.Length; i++)
            {
                LoopListers[i].Init();
                for (int k = 0; k < LoopListers[i].ToUpdate.Count; k++)
                {
                    IUpdater updater = LoopListers[i].ToUpdate[k];
                    AddUpdater(updater);
                }
            }
            UpdateAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updater"></param>
        public void AddUpdater(IUpdater updater)
        {
            if (updater.PlayInFixedUpdate)
            {
                bool added = IUpdatersFixedUpdate.AddIfNotContain(updater);
                if (added)
                {
                    IUpdatersFixedUpdate = LoopUpdater.Order(IUpdatersFixedUpdate);
                }
            }
            else
            {
                bool added = IUpdatersUpdate.AddIfNotContain(updater);
                if (added)
                {
                    IUpdatersUpdate = LoopUpdater.Order(IUpdatersUpdate);
                }
            }
        }

        /// <summary>
        /// we assime it exist
        /// </summary>
        /// <param name="updater"></param>
        public void RemoveUpdater(IUpdater updater)
        {
            if (updater.PlayInFixedUpdate)
            {
                bool removed = IUpdatersFixedUpdate.Remove(updater);
                if (removed)
                {
                    IUpdatersFixedUpdate = LoopUpdater.Order(IUpdatersFixedUpdate);
                }
            }
            else
            {
                bool removed = IUpdatersUpdate.Remove(updater);
                if (removed)
                {
                    IUpdatersUpdate = LoopUpdater.Order(IUpdatersUpdate);
                }
            }
        }


        [Button]
        public void UpdateAll()
        {
            IUpdatersFixedUpdate = LoopUpdater.Order(IUpdatersFixedUpdate);
            IUpdatersUpdate = LoopUpdater.Order(IUpdatersUpdate);
        }

        /// <summary>
        /// called when we change in editor the orders of scripts.
        /// </summary>
        public void SetOrdersFromListFixedUpdate()
        {
           LoopUpdater.SetOrdersFromList(IUpdatersFixedUpdate);
           UpdateAll();
#if UNITY_EDITOR
            SetDirtyAfterChangeOrder();
#endif
        }
        public void SetOrdersFromListUpdate()
        {
            LoopUpdater.SetOrdersFromList(IUpdatersUpdate);
            UpdateAll();
#if UNITY_EDITOR
            SetDirtyAfterChangeOrder();
#endif
        }

#if UNITY_EDITOR
        private void SetDirtyAfterChangeOrder()
        {
            for (int i = 0; i < LoopListers.Length; i++)
            {
                EditorUtility.SetDirty(LoopListers[i]);
            }
        }
#endif

        private static void SetOrdersFromList(List<IUpdater> updaters)
        {
            for (int i = 0; i < updaters.Count; i++)
            {
                if (updaters[i] == null)
                {
                    continue;
                }
                updaters[i].Order = i;
            }
        }

        /// <summary>
        /// Order script, by order
        /// </summary>
        public static List<IUpdater> Order(List<IUpdater> listUpdater)
        {
            for (int i = listUpdater.Count - 1; i >= 1; i--)
            {
                bool sorted = true;
                for (int j = 0; j <= i - 1; j++)
                {
                    if (listUpdater[j + 1] == null || listUpdater[j] == null)
                    {
                        continue;
                    }

                    if (listUpdater[j + 1].Order < listUpdater[j].Order)
                    {
                        listUpdater.Move(j + 1, j);
                        sorted = false;
                    }
                }
                if (sorted)
                    break;
            }
            return (listUpdater);
        }

        /// <summary>
        /// here this function is called in editor AND play, 
        /// if we are in editor, 
        /// play the fixeUpdate ones (to simulate the fixedUpdate ones),
        /// and the update ones.
        /// 
        /// if we are in play, play the update one only
        /// (because the fixed one will be called by fixedUpdate
        /// </summary>
        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (ExecuteEverythingInEditor)
                {
                    EditorUpdater();
                }
            }
            else
            {
                LoopAndExecuteFunction(ExecuteUpdate, IUpdatersUpdate);
            }
        }

        public void EditorUpdater()
        {
            LoopAndExecuteFunction(ExecuteUpdate, IUpdatersFixedUpdate);
            LoopAndExecuteFunction(ExecuteUpdate, IUpdatersUpdate);
        }

        /// <summary>
        /// this function is called in play only
        /// </summary>
        private void FixedUpdate()
        {
            LoopAndExecuteFunction(ExecuteUpdate, IUpdatersFixedUpdate);
        }

        private static void LoopAndExecuteFunction(MethodToCallWhenLooping method, List<IUpdater> updaters)
        {
            for (int k = 0; k < updaters.Count; k++)
            {
                IUpdater updater = updaters[k];

                if (updater == null)
                {
                    continue;
                }

#if UNITY_EDITOR
                if (!updater.CanPlayInEditor && !Application.isPlaying)
                {
                    return;
                }
#endif
                method(updater);
            }
        }

        private void ExecuteUpdate(IUpdater updater)
        {
            updater.CustomLoop();
        }
    }
}