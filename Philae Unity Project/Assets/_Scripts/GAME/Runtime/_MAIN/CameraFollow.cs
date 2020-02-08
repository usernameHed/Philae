using hedCommon.extension.runtime;
using philae.gravity.player;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [ReadOnly]
    public List<PlayerController> Players = new List<PlayerController>(4);
    [SerializeField]
    private Transform _camera;
    [SerializeField]
    private float _speed = 100f;

    private void Start()
    {
        Players.Clear();
        PlayerController[] players = ExtFind.GetScripts<PlayerController>();
        Players = players.ToList();
    }

    private void FixedUpdate()
    {
        Players = ExtList.CleanNullFromList(Players, out bool hasChanged);
        if (Players.Count == 0)
        {
            return;
        }
        Vector3 position = GetMiddlePosition();
        Vector3 moveCamera = Vector3.Lerp(_camera.position, position, Time.fixedDeltaTime * _speed);
        moveCamera.z = _camera.position.z;
        _camera.transform.position = moveCamera;
    }

    private Vector3 GetMiddlePosition()
    {
        List<Vector3> position = new List<Vector3>();
        for (int i = 0; i < Players.Count; i++)
        {
            position.Add(Players[i].Position);
        }
        Vector3 focus = ExtVector3.GetMiddleOfXVector(position.ToArray());
        return (focus);
    }


}
