using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Application.quitting += ReleaseMouse;
        CaptureMouse();
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                CaptureMouse();
            }
        }

        if (Keyboard.current.escapeKey.wasReleasedThisFrame)
        {
            ReleaseMouse();
        }
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
