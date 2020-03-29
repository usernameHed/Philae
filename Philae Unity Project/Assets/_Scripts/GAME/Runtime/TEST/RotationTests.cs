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
            _pointToRotate.position = ExtRotation.RotatePointAroundAxis(_anchor.position, _pointToRotate.position, _anchor.up, _rotateAxis * TimeEditor.deltaTime);

            /*
            Vector3 vectorDirector = _pointToRotate.position - _anchor.position;
            vectorDirector = ExtRotation.RotateVectorAroundAxis(_anchor.position, vectorDirector, _anchor.up, _rotateAxis * TimeEditor.deltaTime);
            _pointToRotate.position = _anchor.position + vectorDirector;
            */
        }
    }
}