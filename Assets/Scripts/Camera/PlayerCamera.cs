using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class PlayerCamera : MonoBehaviour
{
    CinemachineVirtualCamera _camera;
    CinemachineTransposer _transposer;
    [SerializeField] VariableReference<float> panSpeed;
    [SerializeField] VariableReference<float> maxDistanceFromPlayer;
    [SerializeField] float[] lensStates;
    int currLensState;
    Vector2 currentPan;
    bool isPanning;
    private void Awake()
    {
        currentPan = new Vector2();
        _camera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _camera?.GetCinemachineComponent<CinemachineTransposer>();
    }
    private void OnEnable()
    {
        EventsManager.cameraPanInput += OnCameraPanInput;
        EventsManager.changePlayerCameraTarget += OnChangePlayerCameraTarget;
        EventsManager.playerAimInput += OnPlayerAimInput;
    }

    private void OnDisable()
    {
        EventsManager.cameraPanInput -= OnCameraPanInput;
        EventsManager.changePlayerCameraTarget -= OnChangePlayerCameraTarget;
        EventsManager.playerAimInput -= OnPlayerAimInput;
    }
    private void OnCameraPanInput(bool obj)
    {
        if (_camera.m_Follow == null)
            return;

        isPanning = obj;

        if (isPanning)
        {
            Vector2Control mousePositionControl = Mouse.current.position;
            Vector2 mousePosition = mousePositionControl.value;
            Vector2 myPositionOnScreen = Camera.main.WorldToScreenPoint(_camera.m_Follow.position);
            Vector2 offset = mousePosition - myPositionOnScreen;
            currentPan = offset * panSpeed;
            ApplyPan(currentPan);
        }
        else
        {
            ResetPan();
        }
    }
    private void OnPlayerAimInput(Vector2 vector)
    {
        if (!isPanning)
            return;

        if (_camera == null)
            return;

        currentPan += vector * panSpeed;
        ApplyPan(currentPan);
    }
    private void OnChangePlayerCameraTarget(Transform target)
    {
        _camera.Follow = target;
        ResetMouseToCenter();
        ResetPan();
    }
    void ResetMouseToCenter()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height) / 2;
        Mouse.current.WarpCursorPosition(screenSize);
    }
    void ResetPan()
    {
        currentPan = new Vector2();
        ApplyPan(currentPan);
    }
    void ApplyPan(Vector2 offset)
    {
        float currentPanDistance = offset.magnitude;
        if (currentPanDistance > maxDistanceFromPlayer)
        {
            offset.Normalize();
            offset *= maxDistanceFromPlayer;
        }
        _transposer.m_FollowOffset = new Vector3(offset.x, offset.y, -10);
    }

    void OnSetScope(float value)
    {
        int indexTargetDelta = (int)value;

        if (currLensState + indexTargetDelta > 0 && currLensState + indexTargetDelta < lensStates.Length)
        {
            currLensState += indexTargetDelta;
            
        }
        else 
        {
            currLensState = 0;
        }
        _camera.m_Lens.OrthographicSize = lensStates[currLensState];
    }
}
