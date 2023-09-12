using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Dictionary<GameObject, IPools> Pools = new Dictionary<GameObject, IPools>();


    void Start()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Locates or generates a new pool to storing a specific script on a GameObject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objToSpawn"></param>
    /// <param name="pool"></param>
    public void FindPool<T>(GameObject objToSpawn, out Pool<T> pool,string poolName = null) where T : MonoBehaviour, IPoolable<T>
    {
        IPools foundPool;
        if (Pools.TryGetValue(objToSpawn,out foundPool) && foundPool is Pool<T>)
        {
            pool = (Pool<T>)foundPool;
        }
        else
        {
            Pool<T> newPool = new Pool<T>(objToSpawn, transform, poolName);
            Pools.Add(objToSpawn, newPool);
            pool = newPool;
        }
    }
}
