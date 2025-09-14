using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public enum EquipmentType
    {
        Weapon,
        Armor,
        Accessory
    }

    public class Weapon : Equipment
    {
        public ItemInstance ItemInstance { get; private set; }
        public WeaponData WeaponData { get; set; }
        public void Initialize(ItemInstance _itemInstance)
        {
            ItemInstance = _itemInstance;
            WeaponData = ItemInstance.ItemData as WeaponData;
        }
    }
}
