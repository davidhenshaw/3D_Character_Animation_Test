using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager current;

    ObjectPool[] pools;

    private void Awake()
    {
        current = this;
        pools = GetComponents<ObjectPool>();

        if(pools.Length < 1)
        {
            Debug.LogError("There are no pools attached to this GameObject from which to spawn!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SpawnHitVFX(Transform transform)
    {
        //Get hit effect from pool
        PoolableObject vfxObj = pools[0].GetNext(transform);
        //enable the hit effect (it will disable itself)
        vfxObj.enabled = true;
    }

    public void SpawnHitVFX(Vector3 position)
    {
        //Get hit effect from pool
        PoolableObject vfxObj = pools[0].GetNext(position, Quaternion.identity);
        //enable the hit effect (it will disable itself)
        vfxObj.enabled = true;
    }
}
