using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    [SerializeField] UnityEvent[] OnActivate;
    [SerializeField] UnityEvent[] OnDeactivate;
    bool isDown = false;

    public void Activate()
    {
        foreach(UnityEvent ue in OnActivate)
        {
            ue?.Invoke();
        }
    }

    public void Deactivate()
    {
        foreach (UnityEvent ue in OnDeactivate)
        {
            ue?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isDown)
        {
            Activate();
            isDown = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(isDown)
        {
            Deactivate();
            isDown = false;
        }
    }
}
