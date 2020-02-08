using hedCommon.extension.runtime;
using hedCommon.time;
using philae.gravity.graviton;
using philae.gravity.physicsBody;
using philae.gravity.projectile;
using philae.sound;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace philae.gravity.player
{
    public class PlayerGun : MonoBehaviour
    {
        [SerializeField]
        private float _turnRate = 200;
        [SerializeField]
        private float _speedOfFire = 10f;
        [SerializeField]
        private float _offset = 2f;
        [SerializeField]
        private float _rateOfFireInSeconds = 3f;

        [SerializeField]
        private bool _repulseBack = true;
        [SerializeField, EnableIf("_repulseBack")]
        private float _strengthRepulseBack = 5f;

        [SerializeField]
        private Transform _targetToRotate;
        [SerializeField, ReadOnly]
        private Transform _mainReferenceObjectDirection;
        [SerializeField]
        private Graviton _projectilePrefabs;
        [SerializeField]
        private RigidGraviton _rigidGraviton;
        [SerializeField]
        private PlayerController _currentPlayerController;
        [SerializeField]
        private float _maxHoldTime = 2f;
        [SerializeField]
        private PlayerSounds _playerSounds;

        private PlayerControls _controls;
        private bool _hasShoot = false;
        private bool _isLoading = false;

        private Vector2 _rotateInput;
        private Vector3 _lastVectorRelativeDirection;  //last desired rotation
        private FrequencyCoolDown _coolDownBetweenFire = new FrequencyCoolDown();
        private FrequencyChrono _coolDownHoldFire = new FrequencyChrono();



        private void Awake()
        {
            _mainReferenceObjectDirection = Camera.main.transform;

            _controls = new PlayerControls();
        }

        /*
        public void InstantRotate()
        {
            Vector3 dirInput = GetRelativeDirection();
            Quaternion dir = ExtQuaternion.TurretLookRotation(dirInput, _targetToRotate.up);
            DoRotate(dir, _turnRate);
        }
        */

        private void FixedUpdate()
        {
            if (IsRotating())
            {
                _lastVectorRelativeDirection = GetRelativeDirection();
            }
            RotateTarget();

            if (_coolDownBetweenFire.IsNotRunning() && _hasShoot)
            {
                _hasShoot = false;
                SoundManager.Instance.PlaySound(_playerSounds.CoolDownUp);
                SoundManager.Instance.PlaySound(_playerSounds.CanonShoot, false);
                _isLoading = false;
                if (IsHolding())
                {
                    _coolDownHoldFire.StartChrono();
                    SoundManager.Instance.PlaySound(_playerSounds.CanonShoot);
                }
            }
        }

        private bool IsHolding()
        {
            return (false);
        }

        /// <summary>
        /// retourne si le joueur se déplace ou pas
        /// </summary>
        /// <returns></returns>
        public bool IsRotating(float margin = 0)
        {
             return (_rotateInput.magnitude > margin);
        }

        private void RotateTarget()
        {
            Debug.DrawRay(_targetToRotate.position, _lastVectorRelativeDirection * 20, Color.magenta, 1f);
            Quaternion dir = ExtQuaternion.TurretLookRotation2D(_lastVectorRelativeDirection, _targetToRotate.forward);
            DoRotate(dir, _turnRate);
        }

        /// <summary>
        /// get the relative direction 
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRelativeDirection(float xBoost = 1, float yBoost = 1)
        {
            Vector3 dirInput = GetDirInput();
            Vector3 relativeDirection = _mainReferenceObjectDirection.right * dirInput.x * xBoost + _mainReferenceObjectDirection.up * dirInput.y * yBoost;
            return (relativeDirection);
        }

        /// <summary>
        /// get la direction de l'input
        /// </summary>
        /// <returns></returns>
        public Vector2 GetDirInput(bool digital = false)
        {
            float x = _rotateInput.x;
            float y = _rotateInput.y;

            if (digital)
            {
                x = (x > 0f) ? 1 : x;
                x = (x < 0f) ? -1 : x;
                y = (y > 0f) ? 1 : y;
                y = (y < 0f) ? -1 : y;
            }

            Vector2 dirInputPlayer = new Vector2(x, y);
            return (dirInputPlayer);
        }

        private void DoRotate(Quaternion calculatedDir, float speed)
        {
            // Move toward that rotation at a controlled, even speed regardless of framerate.
            _targetToRotate.rotation = Quaternion.RotateTowards(
                                    _targetToRotate.rotation,
                                    calculatedDir,
                                    speed * Time.fixedDeltaTime);
        }

        // Event triggered by PlayerInputManager
        private void OnShootPress()
        {
            _coolDownHoldFire.StartChrono();
            if (_coolDownBetweenFire.IsNotRunning())
            {
                SoundManager.Instance.PlaySound(_playerSounds.CanonShoot);
                _isLoading = true;
            }
            else
            {
                SoundManager.Instance.PlaySound(_playerSounds.CoolDownShootNotReady);
            }
        }

        private void OnShootRelease()
        {
            if (!_isLoading)
            {
                return;
            }

            Debug.Log("Timer released");
            Debug.Log("triggered on shoot hold");
            if (_coolDownBetweenFire.IsNotRunning())
            {
                _hasShoot = true;
                Graviton graviton = Instantiate<Graviton>(_projectilePrefabs, _targetToRotate.position + _targetToRotate.up * _offset, Quaternion.identity, null);
                Projectile projectile = graviton.GetExtComponentInChildrens<Projectile>(99, true);
                projectile.Init(_currentPlayerController);
                Debug.Log("position setupped " + _targetToRotate.position);

                float timer = _coolDownHoldFire.GetTimer();
                timer = Mathf.Clamp(timer, 0, _maxHoldTime);
                Debug.Log(timer);

                graviton.InitialPush.Push(_targetToRotate.up * _speedOfFire * (1 + timer));
                SoundManager.Instance.PlaySound(_playerSounds.CanonShoot, "GUNSHOT RELEASE", 1);

                if (_repulseBack)
                {
                    _rigidGraviton.AddForce(-_targetToRotate.up * _strengthRepulseBack, ForceMode.VelocityChange);
                }
                _isLoading = false;
                _coolDownBetweenFire.StartCoolDown(_rateOfFireInSeconds);
            }
        }

        // Event triggered by PlayerInputManager
        private void OnGunRotate(InputValue value)
        {
            _rotateInput = value.Get<Vector2>();
        }

        private void OnDestroy()
        {
            if (SoundManager.Instance)
            {
                SoundManager.Instance.PlaySound(_playerSounds.CanonShoot, false);
            }
        }
    }
}