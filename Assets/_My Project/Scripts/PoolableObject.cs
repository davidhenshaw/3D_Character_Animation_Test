using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    public event Action<PoolableObject> disabled;

    public virtual void Reset()
    {
        gameObject.SetActive(true);
    }

    public virtual void DisableAndMoveToPool()
    {
        gameObject.SetActive(false);

        if(disabled != null)
            disabled.Invoke(this);
    }

    private void OnDisable()
    {
        disabled.Invoke(this);
    }
}
