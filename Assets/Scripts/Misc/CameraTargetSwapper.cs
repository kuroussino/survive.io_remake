using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetSwapper : MonoBehaviour
{
    public Transform[] targets;
    int targetInput = 0;

    private void OnEnable()
    {
        EventsManager.cameraSwitchInput += OnPlayerSwitchCameraTargetInput;
    }
    private void OnDisable()
    {
        EventsManager.cameraSwitchInput -= OnPlayerSwitchCameraTargetInput;
    }
    private void Start()
    {
        OnPlayerSwitchCameraTargetInput();
    }
    private void OnPlayerSwitchCameraTargetInput()
    {
        targetInput++;
        if (targetInput >= targets.Length)
            targetInput = 0;

        EventsManager.changePlayerCameraTarget?.Invoke(targets[targetInput]);
    }
}
