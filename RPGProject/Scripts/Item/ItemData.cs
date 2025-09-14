using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{

    public enum ItemType
    {
        Equipable,
        Consumable,
    }


    [CreateAssetMenu(fileName = "Item", menuName = "Custom/New Item")]
    public class ItemData : ScriptableObject
    {
        public const int EQUIPMENT_KEY_CONST = 10000;
        public const int CONSUMABLE_KEY_CONST = 20000;

        [field: SerializeField] public Sprite ItemSprite { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public int Key { get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField] public GameObject DropPrefab { get; private set; }

        [field: SerializeField] public bool CanStack { get; private set; }
        [field: SerializeField] public int MaxStackAmount { get; private set; } = 1;

        [field: SerializeField] public AudioClip UsageSoundClip { get; private set; }
        //[Header("Consumable")]
        //public ItemDataForConsumable[] consumables;


    }

}