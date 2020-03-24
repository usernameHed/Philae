using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    [ExecuteInEditMode]
    public class RotationTests : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _rotateAxis;
        [SerializeField]
        private Transform _anchor;
        [SerializeField]
        private Transform _pointToRotate;

        private void Update()
        {
            //Debug.DrawLine(_anchor.position, _pointToRotate.position);
            _pointToRotate.position = ExtRotation.RotatePointAroundAxis(_anchor.position, _pointToRotate.position, _anchor.up, _rotateAxis/* * TimeEditor.deltaTime*/);
        }
    }
}