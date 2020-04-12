using hedCommon.extension.runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParentingBehaviorSettings
{
    public Transform ToMoveLikeAChild;
    public Transform ParentRef;
    public bool AutoMove = true;
}

[ExecuteInEditMode]
public class ParentingBehavior : MonoBehaviour
{
    //this script shouldn't be serialize here
    private ParentingBehaviorSettings _parentingBehaviorSettings;

    private Vector3 _startParentPosition;
    private Quaternion _startParentRotationQ;
    private Vector3 _startParentScale;

    private Vector3 _startChildPosition;
    private Quaternion _startChildRotationQ;

    private Matrix4x4 _parentMatrix;


    /// <summary>
    /// initialize parent, child, and settings
    /// </summary>
    /// <param name="parentingBehaviorSettings"></param>
    public void Init(ref ParentingBehaviorSettings parentingBehaviorSettings)
    {
        _parentingBehaviorSettings = parentingBehaviorSettings;

        _startParentPosition = _parentingBehaviorSettings.ParentRef.position;
        _startParentRotationQ = _parentingBehaviorSettings.ParentRef.rotation;
        _startParentScale = _parentingBehaviorSettings.ParentRef.lossyScale;

        //_startChildPosition = _parentingBehaviorSettings.ToMoveLikeAChild.position;
        //_startChildRotationQ = _parentingBehaviorSettings.ToMoveLikeAChild.rotation;
        _startChildPosition = _parentingBehaviorSettings.ParentRef.position;
        _startChildRotationQ = _parentingBehaviorSettings.ParentRef.rotation;

        // Keeps child position from being modified at the start by the parent's initial transform
        _startChildPosition = ExtVector3.DivideVectors(Quaternion.Inverse(_parentingBehaviorSettings.ParentRef.rotation) * (_startChildPosition - _startParentPosition), _startParentScale);
    }

    private void Update()
    {
        if (_parentingBehaviorSettings == null)
        {
            //Debug.LogWarning("not setup, it's ok if we are in prefabs mode");
            return;
        }
        if (_parentingBehaviorSettings.AutoMove)
        {
            CustomLoop();
        }
    }

    public void CustomLoop()
    {
        BehaveLikeAChild();
    }

    private void BehaveLikeAChild()
    {
        if (_parentingBehaviorSettings == null || _parentingBehaviorSettings.ParentRef == null)
        {
            //Debug.Log("not initialize ?");
            return;
        }

        _parentMatrix = Matrix4x4.TRS(_parentingBehaviorSettings.ParentRef.position, _parentingBehaviorSettings.ParentRef.rotation, _parentingBehaviorSettings.ParentRef.lossyScale);

        _parentingBehaviorSettings.ToMoveLikeAChild.position = _parentMatrix.MultiplyPoint3x4(_startChildPosition);

        _parentingBehaviorSettings.ToMoveLikeAChild.rotation = (_parentingBehaviorSettings.ParentRef.rotation * Quaternion.Inverse(_startParentRotationQ)) * _startChildRotationQ;
    }
}
