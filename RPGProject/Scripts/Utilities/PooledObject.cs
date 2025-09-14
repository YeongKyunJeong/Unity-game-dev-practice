using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

namespace RSP2
{
    public class PooledObject : MonoBehaviour
    {
        public string poolTag;
        private ObjectPool myObjectPooler;

        public void Initialize(ObjectPool objectPool, string tag = "")
        {
            myObjectPooler = objectPool;
            if (poolTag == null)
            {
                poolTag = tag;
            }
        }

        public void ReturnToPool()
        {
            myObjectPooler?.ReturnToPool(poolTag, gameObject);
            if(myObjectPooler == null)
            {
                Debug.Log($"{name} was destroyed");
                Destroy(gameObject);
            }
        }
    }
}
