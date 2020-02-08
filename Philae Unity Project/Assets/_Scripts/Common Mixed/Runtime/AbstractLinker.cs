using hedCommon.singletons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace hedCommon.mixed
{
    public abstract class AbstractLinker : SingletonMono<PhilaeLinker>
    {
        public abstract void InitFromEditor(bool fromPlay);
        public abstract void InitFromPlay();
    }
}