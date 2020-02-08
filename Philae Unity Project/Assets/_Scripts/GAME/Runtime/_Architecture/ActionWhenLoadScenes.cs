using hedCommon.extension.runtime;
using hedCommon.singletons;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace philae.architecture
{
    public class ActionWhenLoadScenes : SingletonMono<ActionWhenLoadScenes>
    {
        public void OnLoadedGame()
        {
            Debug.Log("Load IOnLoadedScene");
            List<IOnLoadedScene> sceneRef = ExtFind.GetInterfaces<IOnLoadedScene>();
            for (int i = 0; i < sceneRef.Count; i++)
            {
                sceneRef[i].OnLoadedSceneEditor();
            }
        }

        //end of class
    }
    //end of namespace
}