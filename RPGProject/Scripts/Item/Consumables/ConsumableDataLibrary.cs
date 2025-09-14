using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class ConsumableDataLibrary
    {
        [field: SerializeField] public List<ConsumableData> ConsumableData { get; private set; }

        public ConsumableData GetConsumableDataCopy(int key)
        {
            if (key >= ConsumableData.Count) return null;

            return Clone(ConsumableData[key]);
        }

        private T Clone<T>(T original) where T : ItemData
        {
            T clone = ScriptableObject.Instantiate(original);
            return clone;
        }
    }
}
