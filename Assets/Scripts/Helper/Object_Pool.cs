using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Object_Pool : Singleton<Object_Pool>
{
    // List of Objcts to ve pooled
    public List<GameObject> prefabForPool;

    // List of the pooled Objects
    private List<GameObject> _pooledObjects = new List<GameObject>();

    public GameObject GetObjectFromPool(string objectName) 
    {
        // try to get a pooled instance
        var instance = _pooledObjects.FirstOrDefault(obj => obj.name == objectName);

        // If we have a pooled instance already
        if (instance != null)
        {
            _pooledObjects.Remove(instance);
            instance.SetActive(true);
            return instance;
        }

        // If we dont have a pooled instance
        var prefab = prefabForPool.FirstOrDefault(obj => obj.name == objectName);
        if (prefab != null) 
        {
            // create a new instance
            var newInstance = Instantiate(prefab,Vector3.zero,Quaternion.identity,transform);
            newInstance.name = objectName;
            return newInstance;
        }

        Debug.LogWarning($"Objec pool dosent have a prefab for the object whit name " + objectName);
        return null;
    }

    public void PoolObject(GameObject obj) 
    {
        obj.SetActive(false);
        _pooledObjects.Add(obj);
    }
}
