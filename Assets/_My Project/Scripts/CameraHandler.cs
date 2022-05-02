using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour, ICameraInputHandler
{
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] float ySensitivity = 0.3f;
    [SerializeField] float maxPitch = 80f;
    [SerializeField] float minPitch = 0;
    [SerializeField] float xSensitivity = 0.3f;

    [SerializeField] bool invertY;

    float pitch = 0;
    float yaw = 0;
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
        _transposer.m_VerticalAxis.m_InputAxisValue = _input.y;
        _transposer.m_HorizontalAxis.m_InputAxisValue = _input.x;
    }
}
