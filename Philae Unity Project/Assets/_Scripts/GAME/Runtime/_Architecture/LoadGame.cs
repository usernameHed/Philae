using philae.architecture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
    public RefGamesAsset RefGameAsset;
    private bool _hasPress = false;

    private void Update()
    {
        if (_hasPress)
        {
            return;
        }

        if (PressA())
        {
            _hasPress = true;
            Load();
        }
    }

    private bool PressA()
    {
        return (true);
    }

    public void Load()
    {
        RefGameAsset.LoadScenesByIndex(0, true, null, false);
    }
}
