using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PlayerCamera : MonoBehaviour
{
    CinemachineVirtualCamera _camera;
    private void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
    }
    private void OnEnable()
    {
        EventsManager.changePlayerCameraTarget += OnChangePlayerCameraTarget;
    }
    private void OnDisable()
    {
        EventsManager.changePlayerCameraTarget -= OnChangePlayerCameraTarget;
    }
    private void OnChangePlayerCameraTarget(Transform transform)
    {
        _camera.Follow = transform;
    }
}
