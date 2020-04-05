using hedCommon.singletons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace hedCommon.sceneWorkflow
{
    public abstract class AbstractLinker : SingletonMono<AbstractLinker>
    {
        public abstract void InitFromEditor(bool fromPlay);
        public abstract void InitFromPlay();
    }
}