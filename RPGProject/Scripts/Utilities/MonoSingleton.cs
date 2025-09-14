using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;
        private static bool applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning($"Singleton Instance '{typeof(T)}' was Deleted. null was Returned");
                    return null;
                }

                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError($"Other Singleton Instance was Detected");
                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject go = new GameObject();
                        instance = go.AddComponent<T>();
                        go.name = $"{typeof(T).Name}";
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        //protected virtual void OnDestroy()
        //{
        //    applicationIsQuitting = true;
        //}
    }

}
