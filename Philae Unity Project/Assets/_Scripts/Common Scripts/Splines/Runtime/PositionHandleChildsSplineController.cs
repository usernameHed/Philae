using hedCommon.extension.runtime;
using FluffyUnderware.Curvy.Controllers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace hedCommon.splines
{
    public class PositionHandleChildsSplineController : MonoBehaviour
    {
        [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
        public bool ShowHandler = true;

        [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
        public List<Transform> _allChildToMove = new List<Transform>();

        [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField, FormerlySerializedAs("_allSplineControllerMove")]
        public List<CustomSplineController> AllSplineControllerMove = new List<CustomSplineController>();

        [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
        public List<Transform> _allChildToRotate = new List<Transform>();

        [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
        public bool canDelete = false;
        [FoldoutGroup("GamePlay"), Tooltip(""), SerializeField]
        public Transform defaultParent;

        /// <summary>
        /// clear fields
        /// </summary>
        public void ResetAll()
        {
            _allChildToMove.Clear();
            _allChildToRotate.Clear();
            Vector3 vec = Vector3.zero;
        }

        public void Init(Transform[] allChildMove, CustomSplineController[] allSplineControllerMove, Transform[] allChildRotate)
        {
            _allChildToMove = ExtList.ToList(allChildMove);
            AllSplineControllerMove = ExtList.ToList(allSplineControllerMove);
            _allChildToRotate = ExtList.ToList(allChildRotate);
        }

        public void InitOnlyMove(Transform[] allChildMove)
        {
            _allChildToMove = ExtList.ToList(allChildMove);
        }

        public void InitOnlyMoveOneByOne(params Transform[] items)
        {
            _allChildToMove = ExtList.ToList(items);
        }


        public void InitOnlySplineControllerMove(CustomSplineController[] allChildMove)
        {
            AllSplineControllerMove = ExtList.ToList(allChildMove);
        }

        public void InitOnlyRotate(Transform[] allChildMove)
        {
            _allChildToRotate = ExtList.ToList(allChildMove);
        }

        public void ClearNullObject()
        {
            _allChildToMove = ExtList.CleanNullFromList(_allChildToMove, out bool valueHasChanged);
            AllSplineControllerMove = ExtList.CleanNullFromList(AllSplineControllerMove, out valueHasChanged);
            _allChildToRotate = ExtList.CleanNullFromList(_allChildToRotate, out valueHasChanged);
        }
    }
}