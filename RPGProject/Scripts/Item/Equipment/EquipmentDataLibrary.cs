using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class EquipmentDataLibrary
    {
        [field: SerializeField] public List<WeaponData> WeaponData { get; private set; }

        public WeaponData GetWeaponDataCopy(int key)
        {
            if (key >= WeaponData.Count) return null;

            return Clone(WeaponData[key]);
        }
             
        private T Clone<T>(T original) where T : EquipmentData
        {
            T clone = ScriptableObject.Instantiate(original);
            return clone;
        }
    }
}
