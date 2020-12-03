using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    float quitTimer;
    [SerializeField] float quitTime = 2;

    void Start()
    {
        Application.quitting += ReleaseMouse;
        //CaptureMouse();
    }

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
            quitTimer = 0;
        }

        if(Keyboard.current.escapeKey.isPressed)
        {
            quitTimer += Time.deltaTime;

            if(quitTimer >= quitTime)
            {
                QuitApplication();
            }
        }
    }

    void QuitApplication()
    {
        Application.Quit();
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
