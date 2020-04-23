using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace hedCommon.crossSceneReference
{
    public class GUIDListerInScene : SerializedMonoBehaviour
    {
        public Dictionary<GuidDescription, GuidComponent> GuidPresentInScene = new Dictionary<GuidDescription, GuidComponent>();

        public GuidComponent GetGUIDFromKey(GuidDescription guidDescription)
        {
            GuidPresentInScene.TryGetValue(guidDescription, out GuidComponent value);
            return (value);
        }
    }
}