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

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        //If the application tries to quit, release the mouse
        Application.quitting += ReleaseMouse;
        CaptureMouse();        
        controls = player.controls;
    }

    // Update is called once per frame
    void Update()
    {
        if(Cursor.lockState != CursorLockMode.Locked)
        {
            if(Mouse.current.leftButton.wasPressedThisFrame)
            {
                CaptureMouse();
            }
        }

        if(Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            ReleaseMouse();
        }
    }

    private void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector2 mouseDelta = controls.Player.CameraLook.ReadValue<Vector2>();
        //Debug.Log(mouseDelta);
        mouseX += mouseDelta.x * verticalSensitivity * Time.deltaTime;
        mouseY -= mouseDelta.y * horizontalSensitivity * Time.deltaTime;

        // rotation must be clamped so you can't look up and behind yourself
        mouseY = Mathf.Clamp(mouseY, -90f, 90f);
        //rotate camera to face player
        

        // Rotate the player about its local y axis based on the mouseX
        player.transform.rotation = Quaternion.Euler(0, mouseX, 0);

        transform.LookAt(target);

        //Apply camera rotation
        transform.Rotate(mouseY, 0, 0);

    }

    void CaptureMouse()
    {
        //Hide cursor and lock it to the window
        //if( !UnityEngine.Debug.isDebugBuild )
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void ReleaseMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
