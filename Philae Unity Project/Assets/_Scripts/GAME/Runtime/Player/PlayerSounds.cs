using philae.sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.player
{
    public class PlayerSounds : MonoBehaviour
    {
        public FmodEventEmitter Boost;
        public FmodEventEmitter CanonShoot;
        public FmodEventEmitter CoolDownUp;
        public FmodEventEmitter GravityCatch;
        public FmodEventEmitter SpaceShipEngine;
        public FmodEventEmitter CoolDownShootNotReady;
    }
}