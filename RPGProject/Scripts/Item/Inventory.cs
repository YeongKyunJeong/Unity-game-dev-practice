using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RSP2
{
    public class ItemInstance
    {
        public ItemData ItemData;
        public int Upgrade;
        public int Amount;
        public event Action<int> AmountChangeEvent;
        public bool isEquipped;

        public ItemInstance(ItemData itemData, int amount = 1, int upgrade = 0)
        {
            this.ItemData = itemData;
            Upgrade = upgrade;
            Amount = amount;
            isEquipped = false;
        }

        public bool Use(int value = 1)
        {
            Amount -= value;
            AmountChangeEvent?.Invoke(Amount);
            return Amount > 0;
        }
    }

    public class Inventory : MonoBehaviour
    {
        private InGameManager gameManager;
        private CanvasUIManager canvasUIManager;
        private InventoryUI inventoryUI;

        private List<ItemInstance> items;

        public void Initialize()
        {
            gameManager = InGameManager.Instance;
            canvasUIManager = CanvasUIManager.Instance;
            inventoryUI = canvasUIManager.PanelUI.InventoryUI;

            items = new List<ItemInstance>();

        }

        public bool AddItem(ItemData itemData, int amount = 1, int upgrade = 0)
        {
            if (amount == 0)
                return false;

            for (int i = 0; i < items.Count; i++)
            {
                ItemInstance item = items[i];
                item.Upgrade = upgrade;

                if (item.ItemData == itemData && itemData.CanStack && itemData.MaxStackAmount > item.Amount)
                {
                    int diff = Mathf.Min(amount, itemData.MaxStackAmount - item.Amount);
                    amount -= diff;
                    item.Amount += diff;

                    inventoryUI.StackItemToUsedSlot(item);

                    if (amount <= 0)
                        return true;
                }
            }

            while (amount > 0)
            {
                int diff = Mathf.Min(amount, itemData.MaxStackAmount);
                if (diff <= 0) break;

                ItemInstance newItem = new ItemInstance(itemData);
                newItem.Amount = diff;
                amount -= diff;

                if (!inventoryUI.AddItemToEmptySlot(newItem))
                {
                    newItem.Amount += amount;
                    Drop(newItem);
                    return false;
                }

                items.Add(newItem);

                if (amount <= 0) return true;
            }

            return false;
        }

        public bool AddItemToSpecificSlot(int slotPosition, ItemData itemData, int upgrade = 0, int amount = 1)
        {
            if (amount == 0) return false;

            ItemInstance newItem = new ItemInstance(itemData);
            newItem.Amount = amount;

            if (inventoryUI.AddItemToSpecificSlot(newItem, slotPosition))
            {
                items.Add(newItem);
                return true;
            }

            return false;
        }

        public bool AddItemToSpecificSlot(ItemInstance item, int slotPosition)
        {
            if (inventoryUI.AddItemToSpecificSlot(item, slotPosition))
            {
                items.Add(item);
                return true;
            }

            return false;
        }

        public void Drop(ItemInstance itemInstance)
        {
            ItemData itemData = itemInstance.ItemData;
            Vector3 dropPosition = transform.position + transform.forward * 1.5f + transform.up * 1.5f;

            GameObject go = Instantiate(itemData.DropPrefab, dropPosition, Quaternion.identity);
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            rigidbody.AddForce(transform.forward * 2, ForceMode.Impulse);

            ItemObject itemObject = go.GetComponent<ItemObject>();
            itemObject.Upgrade = itemInstance.Upgrade;
            itemObject.Amount = itemInstance.Amount;
            if (itemObject.itemData == null)
                itemObject.itemData = itemInstance.ItemData;
        }

        public void RemoveItem(ItemInstance itemInstance)
        {
            if (items.Contains(itemInstance))
                items.Remove(itemInstance);
        }

        public void GetInventoryDataForSave(PlayerSaveData saveData)
        {
            List<ItemSaveData> ItemSaveDataList = new List<ItemSaveData>();

            InventorySlot[] inventorySlots = inventoryUI.GetInventorySlots;

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlot slot = inventorySlots[i];

                if (slot.ItemInstance == null) continue;

                ItemSaveData itemSaveData = new ItemSaveData();
                ItemInstance item = slot.ItemInstance;

                itemSaveData.ItemType = item.ItemData.Type;

                if (itemSaveData.ItemType == ItemType.Equipable)
                {
                    EquipmentData equipmentData = item.ItemData as EquipmentData;
                    itemSaveData.EquipmentType = equipmentData.EquipmentType;
                    itemSaveData.ItemUpgrade = item.Upgrade;
                    itemSaveData.IsEquipped = item.isEquipped; // Should be false;
                }

                itemSaveData.ItemKey = item.ItemData.Key;
                itemSaveData.SlotPosition = i;
                itemSaveData.Amount = item.Amount;

                ItemSaveDataList.Add(itemSaveData);
            }


            InventorySlot[] equipmentSlot = inventoryUI.GetEquipmentSlots;

            for (int i = 0; i < equipmentSlot.Length; i++)
            {
                InventorySlot slot = equipmentSlot[i];

                if (slot.ItemInstance == null) continue;

                ItemSaveData itemSaveData = new ItemSaveData();

                ItemInstance item = slot.ItemInstance;

                itemSaveData.ItemType = ItemType.Equipable;
                EquipmentData equipmentData = item.ItemData as EquipmentData;
                itemSaveData.EquipmentType = equipmentData.EquipmentType;
                itemSaveData.ItemUpgrade = item.Upgrade;
                itemSaveData.IsEquipped = true;

                itemSaveData.ItemKey = item.ItemData.Key;
                itemSaveData.SlotPosition = -i - 1;
                itemSaveData.Amount = 1;

                ItemSaveDataList.Add(itemSaveData);
            }

            saveData.InventoryData = ItemSaveDataList;
        }

        public void EquipBySaveData(ItemInstance equipment)
        {
            //ItemInstance newEquipment = new ItemInstance(equipmentData);

            ////////////
            //(newEquipment.ItemData as EquipmentData).Upgrade = upgrade;

            items.Add(equipment);
            inventoryUI.EquipItemBySave(equipment);
        }

        //public void SetItemsFromSave(PlayerSaveData saveData)
        //{
        //    InventorySlot[] inventorySlots = inventoryUI.GetInventorySlots;

        //    foreach (ItemSaveData itemSaveData in saveData.InventoryData)
        //    {
        //        switch (itemSaveData.ItemType)
        //        {
        //            case ItemType.Equipable:
        //                {
        //                    switch (itemSaveData.EquipmentType)
        //                    {

        //                        case EquipmentType.Weapon:
        //                            {
        //                                WeaponData weaponData = GameManager.Instance.Player.SOData.WeaponDataLibrary.WeaponData[itemSaveData.ItemKey];
        //                                if(itemSaveData.SlotPosition < 0)
        //                                {
        //                                    ItemInstance  
        //                                    break;
        //                                }

        //                                AddItemToSpecificSlot(itemSaveData.SlotPosition, weaponData);
        //                                break;
        //                            }
        //                        case EquipmentType.Armor:
        //                            {
        //                                // TO DO:: Add Logic After Adding Armor
        //                                break;
        //                            }
        //                        case EquipmentType.Accessory:
        //                            {
        //                                // TO DO:: Add Logic After Adding Armor
        //                                break;
        //                            }
        //                    }
        //                    break;
        //                }

        //            case ItemType.Consumable:
        //                {

        //                    break;
        //                }

        //            default:
        //                {

        //                    break;
        //                }
        //        }
        //    }
        //}

    }
}
