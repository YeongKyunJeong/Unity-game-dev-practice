using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Pool;

namespace RSP2
{
    public class ObjectPool : MonoBehaviour
    {

        [System.Serializable]
        public struct Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> availablePoolDictionary;
        public Dictionary<string, HashSet<GameObject>> inUsePoolDictionary;

        private void Start()
        {
            availablePoolDictionary = new Dictionary<string, Queue<GameObject>>();
            inUsePoolDictionary = new Dictionary<string, HashSet<GameObject>>();

            foreach (var pool in pools)
            {
                Queue<GameObject> queue = new Queue<GameObject>();
                HashSet<GameObject> hashSet = new HashSet<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.GetComponent<PooledObject>()?.Initialize(this, pool.tag);
                    obj.SetActive(false);
                    queue.Enqueue(obj);
                }

                availablePoolDictionary.Add(pool.tag, queue);
                inUsePoolDictionary.Add(pool.tag, hashSet);
            }
        }

        public GameObject SpawnFromPool(string poolTag, bool expandable = true)
        {
            if (!availablePoolDictionary.ContainsKey(poolTag))
            {
                Debug.Log($"{poolTag} is not in the Pool Dictionary");
                return null;
            }

            if (!expandable)
            {
                GameObject availableObj = availablePoolDictionary[poolTag].Dequeue();
                availablePoolDictionary[poolTag].Enqueue(availableObj);
                availableObj.SetActive(true);
                return availableObj;
            }

            if (availablePoolDictionary[poolTag].Count > 0)
            {
                GameObject availableObj = availablePoolDictionary[poolTag].Dequeue();
                inUsePoolDictionary[poolTag].Add(availableObj);
                availableObj.SetActive(true);
                return availableObj;
            }

            Pool pool = pools.Find(p => p.tag == poolTag);

            GameObject newObj = Instantiate(pool.prefab);
            newObj.GetComponent<PooledObject>()?.Initialize(this, poolTag);
            inUsePoolDictionary[poolTag].Add(newObj);
            pool.size++;

            return newObj;
        }

        public void ResetPoolDictionary()
        {
            List<string> keys = new List<string>(availablePoolDictionary.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                foreach (var obj in inUsePoolDictionary[keys[i]])
                {
                    obj.SetActive(false);
                    availablePoolDictionary[keys[i]].Enqueue(obj);
                }
                inUsePoolDictionary[keys[i]].Clear();
            }
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            if (!availablePoolDictionary.ContainsKey(tag))
                return ;

            if (inUsePoolDictionary[tag].Remove(obj))
            {
                obj.SetActive(false);
                if (!availablePoolDictionary[tag].Contains(obj))
                {
                    availablePoolDictionary[tag].Enqueue(obj);
                    return ;
                }

                Debug.Log("Object pool return logic error : already existing object in availible pool");
            }

            return ;
        }

    }
}
