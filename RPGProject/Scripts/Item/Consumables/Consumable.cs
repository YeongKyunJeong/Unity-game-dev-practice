using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public enum ConsumableType
    {
        HPHealing,
        MPHealing
    }

    public class Consumable : MonoBehaviour
    {

        public ItemInstance ItemInstance { get; private set; }
        public ConsumableData ConsumableData { get; set; }
        public void Initialize(ItemInstance _itemInstance)
        {
            ItemInstance = _itemInstance;
            ConsumableData = ItemInstance.ItemData as ConsumableData;
        }
    }

    [System.Serializable]
    public class ConsumableEffect
    {
        public ConsumableType ConsumableType;
        public int effectValue;
    }
}
