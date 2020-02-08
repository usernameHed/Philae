using hedCommon.extension.runtime;
using philae.gravity.player;
using philae.sound;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace philae.gravity.projectile
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private PlayerController _playerController;
        [SerializeField]
        private FmodEventEmitter _touch;

        public void Init(PlayerController playerRef)
        {
            _playerController = playerRef;
        }

        private void OnCollisionEnter(Collision collision)
        {

            PlayerController otherPlayer = collision.gameObject.GetExtComponentInParents<PlayerController>(99, true);
            Debug.Log("touch player: " + otherPlayer);
            if (otherPlayer && otherPlayer != _playerController)
            {
                Debug.Log("deleta pleyr !!!");
                otherPlayer.DeletePlayer();
                SoundManager.Instance.PlaySound(_touch);
                Destroy(gameObject, 1f);
                return;
            }
        }
    }
}