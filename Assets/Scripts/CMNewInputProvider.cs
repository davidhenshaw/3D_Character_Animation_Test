using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CMNewInputProvider : MonoBehaviour, AxisState.IInputAxisProvider
{
    CinemachineFreeLook cmFreeLook;
    AxisState axisState;
    [SerializeField] PlayerController playerCtrl;
    [SerializeField] bool invertXAxis;
    [SerializeField] bool invertYAxis;
    InputMaster controls;

    private void Awake()
    {
        cmFreeLook = GetComponent<CinemachineFreeLook>();
        cmFreeLook.m_XAxis.SetInputAxisProvider(0, this);
        cmFreeLook.m_YAxis.SetInputAxisProvider(1, this);
    }

    // Start is called before the first frame update
    void Start()
    {
        controls = playerCtrl.controls;
    }

    public float GetAxisValue(int axis)
    {
        Vector2 input = Vector2.zero;

        if(controls != null)
        {
            input = controls.Player.CameraLook.ReadValue<Vector2>();
            input.Normalize();
        }


        float value = 0;

        switch(axis)
        {
            case 0:
                {
                    value = invertXAxis ? input.x * -1 : input.x;
                    break;
                }
            case 1:
                {
                    value = invertYAxis ? input.y * -1 : input.y;
                    break;
                }
            case 2:
                {
                    Debug.LogError("Cannot get value for axis \"2\". " + name + " does not support Z-Axis inputs");
                    break;
                }
        }

        return value;
    }

}
