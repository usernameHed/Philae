using hedCommon.editorGlobal;
using hedCommon.extension.runtime;
using hedCommon.mixed;
using hedCommon.time;
using philae.architecture;
using philae.gravity.graviton;
using philae.gravity.player;
using philae.gravity.zones;
using philae.sound;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhilaeLinker : AbstractLinker
{
    [FoldoutGroup("Object")]
    public LoopUpdater LoopUpdater;
    [FoldoutGroup("Object")]
    public SoundManager SoundManager;

    public RefGamesAsset RefGameAsset;
    private bool _hasInitialize = false;

    public override void InitFromEditor(bool fromPlay)
    {
        if (ZonesLister.Instance == null || GravitonLister.Instance == null)
        {
            Debug.Log("game unloading...");
            return;
        }

        ManuallyAddGravitonInEditor();
        InitFromPlay();
    }

    public override void InitFromPlay()
    {
        if (Application.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(WaitBeforeLaunch());
            return;
        }
        else
        {
            if (ZonesLister.Instance == null || GravitonLister.Instance == null)
            {
                Debug.Log("game unloading...");
                return;
            }
            FinalizeInitGame();
        }
    }

    private IEnumerator WaitBeforeLaunch()
    {
        bool isOk = ZonesLister.Instance != null && GravitonLister.Instance != null;
        Debug.Log(isOk);
        yield return new WaitUntil(() => isOk);
        FinalizeInitGame();
    }

    private void FinalizeInitGame()
    {
        SoundManager.Instance.Init();
        ZonesLister.Instance.Init();
        GravitonLister.Instance.Init(ZonesLister.Instance);
        ExtLog.Log("Load everything here", Color.magenta);
        LoopUpdater.Init();

        if (Application.isPlaying)
        {
            //initialize on play
            _hasInitialize = true;
        }

    }

    public void ManuallyAddGravitonInEditor()
    {
        if (Application.isPlaying)
        {
            return;
        }
        Debug.Log("manually Add All Graviton");
        Graviton[] allGraviton = ExtFind.GetScripts<Graviton>();
        for (int i = 0; i < allGraviton.Length; i++)
        {
            allGraviton[i].AddToLister();
        }
    }

    public void RestartGame()
    {
        Debug.Log("restart game here");
        RefGameAsset.LoadScenesByIndex(0, true, null, false);
    }
}
