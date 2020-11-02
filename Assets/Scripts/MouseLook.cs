using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float verticalSensitivity = 64f;
    [SerializeField] float horizontalSensitivity = 64f;
    InputMaster controls;
    float mouseX;
    float mouseY;
    [SerializeField] PlayerController player;
    [SerializeField] Transform target;
    [SerializeField] float maxVerticalAngle = 90;
    [SerializeField] float minVerticalAngle = -90;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {    
        controls = player.controls;
    }

    private void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            RotatePlayer();
        }
    }

    void RotatePlayer()
    {
        // Rotate the player about its local y axis based on the mouseX
        player.transform.rotation = Quaternion.Euler(0, mouseX, 0);
    }

}
