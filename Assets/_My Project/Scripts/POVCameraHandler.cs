using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class POVCameraHandler : MonoBehaviour, ICameraInputHandler
{
    [SerializeField] CinemachineVirtualCamera vCam;
    [Range(1, 3)]
    [SerializeField] float xSensitivity = 1f;
    [Range(1, 3)]
    [SerializeField] float ySensitivity = 1f;

    CinemachinePOV _transposer;

    Vector2 _input;

    private void Awake()
    {
        _transposer = vCam.GetCinemachineComponent<CinemachinePOV>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnCameraLook(InputValue value)
    {
        _input = value.Get<Vector2>();   
    }

    private void LateUpdate()
    {
        _transposer.m_VerticalAxis.m_InputAxisValue = _input.y * ySensitivity;
        _transposer.m_HorizontalAxis.m_InputAxisValue = _input.x * xSensitivity;
    }
}
