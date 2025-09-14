using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum PoolObjectType
{
    Enemy
}

[System.Serializable]
public class Pool
{
    public PoolObjectType key;
    public GameObject prefab;
    public int size;
}

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private List<Pool> pools = new List<Pool>();
    private Pool tempPool;
    private Dictionary<PoolObjectType, Pool> typeToPoolDictionary;
    private List<GameObject> pooledGameObjects;
    private GameObject newPooledGameObject;
    private Dictionary<PoolObjectType, List<GameObject>> typeToPooledGameObjectsDictionary;

    public void Initialize()
    {
        typeToPooledGameObjectsDictionary = new Dictionary<PoolObjectType, List<GameObject>>();
        typeToPoolDictionary = new Dictionary<PoolObjectType, Pool>();

        for (int i = 0; i < pools.Count; i++)
        {
            typeToPooledGameObjectsDictionary[pools[i].key] = new List<GameObject>();
            typeToPoolDictionary[pools[i].key] = pools[i];
        }
    }

    public void Check()
    {

    }

    public T GetObject<T>(PoolObjectType key) where T : MonoBehaviour, IPoolableObject
    {
        if (typeToPooledGameObjectsDictionary.TryGetValue(key, out pooledGameObjects))
        {
            for (int i = 0; i < pooledGameObjects.Count; i++)
            {
                if (!pooledGameObjects[i].activeInHierarchy)
                {
                    T found_T = pooledGameObjects[i].GetComponent<T>();
                    found_T.EachPoolingInitialize();
                    return found_T;
                }
            }


            tempPool = typeToPoolDictionary[key];


            tempPool.size++;
            newPooledGameObject = Instantiate(tempPool.prefab);
            pooledGameObjects.Add(newPooledGameObject);

            T new_T = newPooledGameObject.GetComponent<T>();
            new_T.FirstPoolingInitialize();
            return new_T;

        }
        return null;
    }
}