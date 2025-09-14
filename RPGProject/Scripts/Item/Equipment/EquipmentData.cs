using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class EquipmentData : ItemData
    {
        public const int WEAPON_KEY_CONTS = 1000;
        public const int ARMOR_KEY_CONTS = 2000;
        public const int ACCESSORY_KEY_CONTS = 3000;

        [field: SerializeField] public EquipmentType EquipmentType { get; private set; }
        //[field: SerializeField] public int Upgrade { get; private set; } = 0;
    }
}
