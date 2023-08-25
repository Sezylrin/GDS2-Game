using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable<T> where T : MonoBehaviour, IPoolable<T>
{
    public Pool<T> Pool { get; set; }
    /// <summary>
    /// Implement a function to add itself back into the pool.
    /// Use to replace destroy function in execution
    /// </summary>
    public void poolSelf();

}

public interface IPools
{

}
[System.Serializable]
public class Pool<T> : IPools where T : MonoBehaviour, IPoolable<T>
{
    [SerializeField]
    private List<T> pooledObj = new List<T>();
    private GameObject objToSpawn;
    private Transform poolParent;
    public Pool(GameObject objToSpawn, Transform parent)
    {
        this.objToSpawn = objToSpawn;
        poolParent = new GameObject(typeof(T).ToString() + " Pool").transform;
        poolParent.transform.SetParent(parent);
    }
    /// <summary>
    /// Returns an instance of the store type in the pool
    /// </summary>
    /// <returns></returns>
    public T GetPooledObj()
    {
        if (pooledObj.Count > 0)
        {
            pooledObj[0].gameObject.SetActive(true);
            T ScriptToReturn = pooledObj[0];
            pooledObj.RemoveAt(0);
            return ScriptToReturn;
        }
        else
        {
            GameObject spawnedObj = Object.Instantiate(objToSpawn, poolParent);
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
        pooledObj.Add(objToPool);
        objToPool.gameObject.SetActive(false);
    }
}
