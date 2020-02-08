using ExtUnityComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static philae.gravity.physicsBody.PhysicBody;

namespace hedCommon.tools
{
    public class LockPositionXYZ : MonoBehaviour, IEditorOnly
    {
        public Component GetReference()
        {
            return (this);
        }

        [SerializeField]
        private ConstrainPosition _constrains;
    }
}