using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace extUnityComponents
{
    /// <summary>
    /// Have to be set on top in the scriptExecutionOrder !
    /// 
    /// Remove Components marked as EditorOnly, and execute actions when you building your game
    /// ExecuteBuildCleaning is called thanks to BuildCallBack().
    /// This function will also get called when entering Playmode, when SceneManager.LoadScene is called.
    /// if you want to desactivate this behavior, set PlayModeCleaning to false
    /// </summary>
    public class EditorOnlyCleaning : MonoBehaviour
    {
        public bool PlayModeCleaning = true;    //active the cleaning when entering in play mode
        public bool BuildCleaning = true;       //active the cleaning when building

        //all action you want to do
        public List<UnityEvent> AllActions = new List<UnityEvent>();
        //all Component you want to delete
        public List<Component> AllBehaviorToDeleteFromBuild = new List<Component>();

        /// <summary>
        /// - Call all action
        /// - Delete all behavior
        /// - Delete itself
        /// </summary>
        public void ExecuteBuildCleaning()
        {
            if (!BuildCleaning || (Application.isPlaying && !PlayModeCleaning))
            {
                return;
            }
            ExecuteUnityEventAction();
            RemoveAllComponents();
            RemoveItself();
        }

        /// <summary>
        /// execute all UnityEvent action
        /// We theoricly could have only one UnityEvent with multiple action,
        /// but then we can't prediect the order of actions.
        /// Here with a list of UnityEvent, we manage the order ourself.
        /// </summary>
        private void ExecuteUnityEventAction()
        {
            for (int i = 0; i < AllActions.Count; i++)
            {
                AllActions[i].Invoke();
            }
        }

        /// <summary>
        /// Delete every components in the list
        /// </summary>
        private void RemoveAllComponents()
        {
            for (int i = 0; i < AllBehaviorToDeleteFromBuild.Count; i++)
            {
                if (AllBehaviorToDeleteFromBuild[i] == null)
                {
                    continue;
                }

                if (Application.isPlaying)
                {
                    Destroy(AllBehaviorToDeleteFromBuild[i]);
                }
                else
                {
                    DestroyImmediate(AllBehaviorToDeleteFromBuild[i]);
                }
            }
        }

        /// <summary>
        /// Remove the script component this
        /// </summary>
        private void RemoveItself()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }
    }
}