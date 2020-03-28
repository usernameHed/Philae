using hedCommon.time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hedCommon.extension.runtime
{
    [ExecuteInEditMode]
    public class RotationTests2 : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _rotateAxis;
        [SerializeField]
        private Transform _anchor;

        private void Update()
        {
            //_pointToRotate.position = ExtRotation.RotatePointAroundAxis(_anchor.position, _pointToRotate.position, _anchor.up, _rotateAxis * TimeEditor.deltaTime);
            _anchor.rotation = ExtRotation.RotateVectorDirectorFromAxis(_anchor.forward, _anchor.up, _rotateAxis * TimeEditor.deltaTime);
            ExtDrawGuizmos.DebugArrowConstant(_anchor.position, _anchor.up, Color.green);
            ExtDrawGuizmos.DebugArrowConstant(_anchor.position, _anchor.forward, Color.blue);
            ExtDrawGuizmos.DebugArrowConstant(_anchor.position, _anchor.right, Color.red);
        }
    }
}