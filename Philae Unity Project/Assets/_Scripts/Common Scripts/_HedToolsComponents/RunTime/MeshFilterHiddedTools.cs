using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extUnityComponents
{
    [ExecuteInEditMode]
    public class MeshFilterHiddedTools : MonoBehaviour, IEditorOnly
    {
        [SerializeField]
        private Vector3 _pivotReferencePosition = Vector3.zero;
        [SerializeField]
        private Quaternion _pivotReferenceRotation = Quaternion.identity;

        public bool SnapVertex = false;
        public bool LimitBounds = false;

        private void Awake()
        {
            this.hideFlags = HideFlags.HideInInspector;
            //this.hideFlags = HideFlags.None;
        }

        public Component GetReference()
        {
            return (this);
        }

        public void Init()
        {
            
        }


        public Vector3 GetPivotReferencePosition()
        {
            return (_pivotReferencePosition);
        }

        public Quaternion GetPivotReferenceRotation()
        {
            return (_pivotReferenceRotation);
        }

        public void SaveDefinitivePivot(Vector3 localPositionTmpPivot, Quaternion rotation)
        {
            _pivotReferencePosition = localPositionTmpPivot;
            _pivotReferenceRotation = rotation;
        }

        public void ResetPivot()
        {
            _pivotReferencePosition = Vector3.zero;
        }
    }
}