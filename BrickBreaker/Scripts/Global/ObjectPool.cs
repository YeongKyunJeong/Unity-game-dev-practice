using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PoolObjectType
{
    Ball,
    DroppingItemBox
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
        Ball[] preGeneratedBalls = GameObject.FindObjectsByType<Ball>(FindObjectsSortMode.None);

        typeToPooledGameObjectsDictionary = new Dictionary<PoolObjectType, List<GameObject>>();
        typeToPoolDictionary = new Dictionary<PoolObjectType, Pool>();

        for (int i = 0; i < pools.Count; i++)
        {
            typeToPooledGameObjectsDictionary[pools[i].key] = new List<GameObject>();
            typeToPoolDictionary[pools[i].key] = pools[i];
            if (pools[i].key == PoolObjectType.Ball)
            {
                if (preGeneratedBalls.Length > 0)
                {
                    pools[i].size = preGeneratedBalls.Length;
                    for (int j = 0; j < preGeneratedBalls.Length; j++)
                    {
                        typeToPooledGameObjectsDictionary[PoolObjectType.Ball].Add(preGeneratedBalls[j].gameObject);
                    }
                }
                Debug.Log($"Detected balls : {preGeneratedBalls.Length}");
            }
        }
    }

    public void Check()
    {

    }

    //void Start()
    //{
    //    foreach (var pool in pools)
    //    {
    //        poolDictionary[pool.key] = new List<GameObject>();
    //        for (int i = 0; i < pool.size; i++)
    //        {
    //            GameObject go = Instantiate(pool.prefab);
    //            go.SetActive(false);
    //            poolDictionary[pool.key].Add(go);
    //        }
    //    }
    //}

    public T GetObject<T>(PoolObjectType key) where T : MonoBehaviour, IPoolMemberObject
    {
        if (typeToPooledGameObjectsDictionary.TryGetValue(key, out pooledGameObjects))
        {
            for (int i = 0; i < pooledGameObjects.Count; i++)
            {
                if (!pooledGameObjects[i].activeInHierarchy)
                {
                    T found_T = pooledGameObjects[i].GetComponent<T>();
                    found_T.RelInitialize();
                    return found_T;
                }
            }


            tempPool = typeToPoolDictionary[key];


            tempPool.size++;
            newPooledGameObject = Instantiate(tempPool.prefab);
            pooledGameObjects.Add(newPooledGameObject);

            T new_T = newPooledGameObject.GetComponent<T>();
            new_T.Initialize();
            return new_T;


            //foreach (Pool pool in pools)
            //{
            //    if (pool.key == key)
            //    {
            //        pool.size++;
            //        newPooledGameObject = Instantiate(pool.prefab);
            //        pooledGameObjects.Add(newPooledGameObject);

            //        T new_T = newPooledGameObject.GetComponent<T>();
            //        new_T.Initialize();
            //        return new_T;
            //    }
            //}
        }
        return null;
    }

    // public T Add<T>(T a, T b)
    // {
    //     return a + b;
    // }
    //
    // public int Add(int a, int b)
    // {
    //     return a + b;
    // }
    //
    // public float Add(float a, float b)
    // {
    //     return a + b;
    // }
    //
    // public void Test()
    // {
    //     // 오버로드
    //     Add(1, 1);
    //     Add(1.0f, 1.0f);
    //     
    // }
}

// public class MonoSingleton<TestManager> : MonoBehaviour where T : MonoSingleton<T>
// public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
// {
//     private static T instance;
//     public static T Instance{get { return instance; }}
//
//     public void SetInstance(T instance)
//     {
//         MonoSingleton<T>.instance = instance;
//     }
// }
//
// public class TestManager : MonoSingleton<TestManager>
// {
//     public void Start()
//     {
//         SetInstance(this);
//     }
// }
//
// public class GameMgr : MonoBehaviour
// {
//     private static GameMgr instance;
//     public static GameMgr Instance{get { return instance; }}
//
//     public static GameMgr GetInstance()
//     {
//         return instance;
//     }
//     
//     private void Awake()
//     {
//         instance = this;
//     }
// }


// public class TestManager2 : MonoSingleton<TestManager>
// {
//     public void Start()
//     {
//         instance = this;
//     }
// }













