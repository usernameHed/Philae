using hedCommon.extension.runtime;
using philae.sound;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.physicsBody
{
    [ExecuteInEditMode]
    public class RigidAttractor : PhysicBody
    {
        [SerializeField]
        private FmodEventEmitter _impact;

        private void OnCollisionEnter(Collision collision)
        {
            SoundManager.Instance.PlaySound(_impact);
        }
    }
}