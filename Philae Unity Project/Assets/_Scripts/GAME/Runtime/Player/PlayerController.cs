using hedCommon.extension.runtime;
using philae.gravity.physicsBody;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace philae.gravity.player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private RigidGraviton _graviton;
        public Vector3 Position { get { return (_graviton.Position); } }

        [SerializeField]
        private float _speed = 200;

        private Vector2 _move;
        private Vector2 _forceInput;

        private void OnMove(InputValue value)
        {
            _move = value.Get<Vector2>();
        }

        private void FixedUpdate()
        {
            _forceInput = new Vector2(_move.x, _move.y) * Time.fixedDeltaTime * _speed;
            if (_forceInput != Vector2.zero)
            {
                _graviton.AddForce(_forceInput, ForceMode.Force);
            }
        }

        private void OnDrawGizmos()
        {
            if (_forceInput != Vector2.zero)
            {
                ExtDrawGuizmos.DrawArrow(_graviton.Position, _forceInput, Color.magenta);
            }
        }

        public void DeletePlayer()
        {
            Destroy(gameObject);
        }
    }
}