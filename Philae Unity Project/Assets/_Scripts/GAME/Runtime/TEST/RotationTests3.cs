using hedCommon.extension.runtime.range;
using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    [ExecuteInEditMode]
    public class RotationTests3 : MonoBehaviour
    {
        [SerializeField]
        private Transform _toRotate;
        [SerializeField]
        private Transform _toRotateUp;
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private float _speedTurret = 100f;
        [SerializeField]
        private float _speedGun = 100f;

        [SerializeField, Range(-180, 0)]
        private float _left = -180;
        [SerializeField, Range(0, 180)]
        private float _right = 180;

        [SerializeField, Range(0, 180)]
        private float _up = 180;
        [SerializeField, Range(-180, 0)]
        private float _down = -180;

        private void Update()
        {
            //RotateTowardTarget();
            RotateUp();
        }

        private void RotateTowardTarget()
        {
            Vector3 finalHeadForward = _target.transform.position - _toRotate.transform.position;
            _toRotate.rotation = ExtRotation.SmoothTurretLookRotation(finalHeadForward, transform.up, _toRotate.rotation, _speedTurret);
            
            Debug.DrawRay(_toRotate.position, _toRotate.up * 0.5f, Color.green);
            Debug.DrawRay(_toRotate.position, _toRotate.forward * 0.5f, Color.blue);
            Debug.DrawRay(_toRotate.position, _toRotate.right * 0.5f, Color.red);
        }

        private void RotateUp()
        {
            Vector3 finalHeadForward = _target.transform.position - _toRotateUp.transform.position;
            _toRotateUp.rotation = ExtRotation.SmoothTurretLookRotationWithClampedAxis(_toRotate.rotation, finalHeadForward, _left, _right, _up, _down, _toRotateUp.rotation, _speedGun);

            ExtDrawGuizmos.DebugArrowConstant(_toRotateUp.position, _toRotateUp.up * 0.2f, Color.green, 0.05f);
            ExtDrawGuizmos.DebugArrowConstant(_toRotateUp.position, _toRotateUp.forward * 0.2f, Color.blue, 0.05f);
            ExtDrawGuizmos.DebugArrowConstant(_toRotateUp.position, _toRotateUp.right * 0.2f, Color.red, 0.05f);
        }
    }
}