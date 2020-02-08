using hedCommon.extension.runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class SceneRef : MonoBehaviour
{
    [SerializeField]
    protected ExtSceneReference _currentScene = new ExtSceneReference();
    public ExtSceneReference GetSceneReference { get { return (_currentScene); } }

    private void Awake()
    {
        Scene scene = gameObject.scene;
        _currentScene.ScenePath = scene.path;
        gameObject.name = "___________________ref " + _currentScene.SceneName;
        gameObject.transform.SetAsFirstSibling();
        gameObject.hideFlags = HideFlags.NotEditable;
    }
}
