using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable<T> where T : MonoBehaviour, IPoolable<T>
{
    public Pool<T> Pool { get; set; }

    public bool IsPooled { get; set; }
    /// <summary>
    /// Implement a function to add itself back into the pool.
    /// Use to replace destroy function in execution
    /// </summary>
    public void PoolSelf();

}

public interface IPools
{

}
[System.Serializable]
public class Pool<T> : IPools where T : MonoBehaviour, IPoolable<T>
{
    [SerializeField]
    private Stack<T> pooledObj = new Stack<T>();
    private GameObject objToSpawn;
    private Transform poolParent;
    public Pool(GameObject objToSpawn, Transform parent, string poolName)
    {
        this.objToSpawn = objToSpawn;
        poolParent = new GameObject(typeof(T).ToString() + " Pool").transform;
        if (poolName != null)
        {
            poolParent.name = poolName + " Pool";
        }
        poolParent.transform.SetParent(parent);
    }
    /// <summary>
    /// Returns an instance of the store type in the pool
    /// </summary>
    /// <returns></returns>
    public T GetPooledObj(out bool newInstantiate)
    {
        if (pooledObj.Count > 0)
        {
            T ScriptToReturn = pooledObj.Pop();
            ScriptToReturn.gameObject.SetActive(true);
            ScriptToReturn.IsPooled = false;
            newInstantiate = false;
            return ScriptToReturn;
        }
        else
        {
            GameObject spawnedObj = UnityEngine.Object.Instantiate(objToSpawn, poolParent);
            spawnedObj.SetActive(true);
            T script = spawnedObj.GetComponentInChildren<T>();
            if (script == null)
            {
                Debug.LogError("The given objToSpawn does not contain the required Script to store");
                Debug.Break();
                newInstantiate = false;
                return null;
            }
            script.Pool = this;
            newInstantiate = true;
            return script;
        }
    }
    public T GetPooledObj()
    {
        if (pooledObj.Count > 0)
        {
            T ScriptToReturn = pooledObj.Pop();
            ScriptToReturn.gameObject.SetActive(true);
            ScriptToReturn.IsPooled = false;
            return ScriptToReturn;
        }
        else
        {
            GameObject spawnedObj = UnityEngine.Object.Instantiate(objToSpawn, poolParent);
            spawnedObj.SetActive(true);
            T script = spawnedObj.GetComponentInChildren<T>();
            if (script == null)
            {
                Debug.LogError("The given objToSpawn does not contain the required Script to store");
                Debug.Break();
                return null;
            }
            script.Pool = this;
            return script;
        }
    }

    public void PoolObj(T objToPool)
    {
        if (objToPool.IsPooled)
            return;
        objToPool.IsPooled = true;
        pooledObj.Push(objToPool);
        objToPool.gameObject.SetActive(false);
    }
}
