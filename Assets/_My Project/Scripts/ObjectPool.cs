using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] PoolableObject objectPrefab;
    protected List<PoolableObject> pool;
    protected Queue<PoolableObject> queue;
    //Events
    public event Action ObjectEnqueued;
    public event Action ObjectDequeued;

    public int PoolCount { get => pool.Count; }

    public virtual void Awake()
    {
        queue = new Queue<PoolableObject>();
        pool = new List<PoolableObject>();
    }

    public virtual bool HasNext()
    {
        return queue.Count > 0;
    }

    public virtual PoolableObject GetNext(Vector3 position, Quaternion rotation)
    {
        PoolableObject objectInstance;

        if(queue.Count > 0)
        {//Get an object from the pool
            objectInstance = queue.Dequeue();
            ObjectDequeued?.Invoke();
        }
        else
        {// Make a new object
            objectInstance = GrowPool();
        }

        objectInstance.Reset();
        objectInstance.transform.position = position;
        objectInstance.transform.rotation = rotation;

        return objectInstance;
    }

    public virtual PoolableObject GetNext(Transform tf)
    {
        PoolableObject objectInstance;

        if (queue.Count > 0)
        {//Get an object from the pool
            objectInstance = queue.Dequeue();
            ObjectDequeued?.Invoke();
        }
        else
        {// Make a new object
            objectInstance = GrowPool();
        }

        objectInstance.Reset();
        objectInstance.transform.position = tf.position;
        objectInstance.transform.rotation = tf.rotation;

        return objectInstance;
    }

    public virtual PoolableObject GetNext()
    {
        PoolableObject objectInstance;

        if (queue.Count > 0)
        {//Get an object from the pool
            objectInstance = queue.Dequeue();
            ObjectDequeued?.Invoke();
        }
        else
        {// Make a new object
            objectInstance = GrowPool();
        }

        objectInstance.Reset();
        objectInstance.transform.position = Vector3.zero;
        objectInstance.transform.rotation = Quaternion.identity;

        return objectInstance;
    }

    public PoolableObject GrowPool()
    {
        PoolableObject newInstance = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity);
        pool.Add(newInstance);
        newInstance.disabled += AddToQueue;

        return newInstance;
    }

    public void AddToQueue(PoolableObject poolableObject)
    {
        queue.Enqueue(poolableObject);
        ObjectEnqueued?.Invoke();
    }

    public Queue<PoolableObject> GetQueue()
    {
        return queue;
    }

    public PoolableObject[] GetActiveInstances()
    {
        List<PoolableObject> activeInstances = new List<PoolableObject>();

        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].isActiveAndEnabled)
            {
                activeInstances.Add(pool[i]);
            }
        }

        return activeInstances.ToArray();
    }

    public PoolableObject[] GetPool()
    {
        return pool.ToArray();
    }

}
