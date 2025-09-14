using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace RSP2
{
    public class InventoryUI : MonoBehaviour, IDropHandler
    {
        private InGameManager gameManager;
        private CanvasUIManager uIManager;
        private PanelUI panelUI;
        private Player player;


        [field: SerializeField] private GameObject itemSlotPrefab;
        [field: SerializeField] private Transform contentRoot;


        [field: SerializeField] private InventorySlot[] inventorySlots;
        public InventorySlot[] GetInventorySlots { get { return inventorySlots; } }

        [field: SerializeField] private InventorySlot selectedItemInSlot;
        [field: SerializeField] private MovingSlot movingSlot;

        [field: SerializeField] private InventorySlot weaponSlot;
        [field: SerializeField] private InventorySlot armorSlot;
        [field: SerializeField] private InventorySlot accessorySlot;
        private InventorySlot[] equipmentSlots;
        public InventorySlot[] GetEquipmentSlots { get { return equipmentSlots; } }


        [field: SerializeField] private Button equipButton;
        [field: SerializeField] private Button useButton;
        [field: SerializeField] private Button dropButton;

        private bool isDragging;


        public bool IsActive { get => GetIsOpened(); }

        public void Initialize(InGameManager _gameManager, CanvasUIManager _uIManager, PanelUI _panelUI)
        {
            player = _gameManager.Player;
            uIManager = _uIManager;
            panelUI = _panelUI;
            panelUI.PointerDropEvent += OnBackGroundDrop;

            gameObject.SetActive(false);

            equipButton.onClick.AddListener(OnEquipButton);
            useButton.onClick.AddListener(OnUseButton);
            dropButton.onClick.AddListener(OnDropButton);

            foreach (var slot in inventorySlots)
            {
                slot.Initialize(this);
                slot.DragBeginEvent += OnBeginInventorySlotDrag;
                slot.ClickEvent += OnInventorySlotClick;
                slot.PointerDropEvent += OnInventorySlotPointerDrop;
            }
            movingSlot.Initialize(this);

            equipmentSlots = new InventorySlot[3];
            equipmentSlots[0] = weaponSlot;
            equipmentSlots[1] = armorSlot;
            equipmentSlots[2] = accessorySlot;

            foreach (var slot in equipmentSlots)
            {
                slot.Initialize(this);
                slot.DragBeginEvent += OnBeginInventorySlotDrag;
                slot.PointerDropEvent += OnEquipmentSlotPointerDrop;
            }

            isDragging = false;
        }

        private bool GetIsOpened()
        {
            return gameObject.activeSelf;
        }

        public void Open()
        {
            if (selectedItemInSlot != null)
            {
                selectedItemInSlot.SetActiveOfSelectedFrame(false);
            }
            selectedItemInSlot = null;
            UpdateButtons(null);
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public bool AddItemToEmptySlot(ItemInstance item)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].ItemInstance == null)
                {
                    InventorySlot slot = inventorySlots[i];
                    slot.Initialize(this);
                    slot.SetItem(item);

                    return true;
                }
            }

            return false;

        }
        public bool AddItemToSpecificSlot(ItemInstance item, int slotPosition)
        {
            if (slotPosition > inventorySlots.Length) return false;

            if (inventorySlots[slotPosition].ItemInstance == null)
            {
                inventorySlots[slotPosition].SetItem(item);
                return true;
            }

            return false;
        }

        public bool StackItemToUsedSlot(ItemInstance item)
        {
            InventorySlot emptySlot = inventorySlots.First(slot => slot.ItemInstance == item);

            if (emptySlot == null) return false;

            emptySlot.SetItem(item);
            return true;
        }


        public void SelectItem(InventorySlot slot)
        {
            selectedItemInSlot = slot;
            UpdateButtons(slot.ItemInstance);
        }

        public void UpdateButtons(ItemInstance item)
        {
            if (item == null)
            {
                useButton.interactable = false;
                equipButton.interactable = false;
                dropButton.interactable = false;
                return;
            }

            switch (item.ItemData.Type)
            {
                case ItemType.Consumable:
                    useButton.interactable = true;
                    equipButton.interactable = false;
                    break;
                case ItemType.Equipable:
                    useButton.interactable = false;
                    equipButton.interactable = true;
                    break;
            }
            dropButton.interactable = true;
        }

        public void OnUseButton()
        {
            if (selectedItemInSlot == null) return;

            ConsumableData ConsumableData = selectedItemInSlot.ItemInstance.ItemData as ConsumableData;

            if (ConsumableData == null) return;

            for (int i = 0; i < ConsumableData.ConsumableEffects.Length; i++)
            {
                switch (ConsumableData.ConsumableEffects[i].ConsumableType)
                {
                    case ConsumableType.HPHealing: // TODO :: Make separate healing logic
                        {
                            player.CombatSystem.ChangeHealth(ConsumableData.ConsumableEffects[i].effectValue);
                            break;
                        }
                }
            }

            SFXManager.PlayClip(ConsumableData.UsageSoundClip, player.transform.position);

            if (selectedItemInSlot.ItemInstance.Use() == false)
            {
                selectedItemInSlot.ClearSlot(true);
                Destroy(selectedItemInSlot.gameObject);
                selectedItemInSlot = null;
                UpdateButtons(null);
            }
            else
            {
                selectedItemInSlot.SetItem(selectedItemInSlot.ItemInstance);
            }
        }

        public void OnEquipButton()
        {
            if (selectedItemInSlot == null) return;

            EquipmentData equipmentData = selectedItemInSlot.ItemInstance.ItemData as EquipmentData;

            if (equipmentData == null)
            {
                selectedItemInSlot.SetItem(movingSlot.ItemInstance);
                movingSlot.DropItem();
                selectedItemInSlot.SetActiveOfSelectedFrame(true);

                UpdateButtons(selectedItemInSlot.ItemInstance);
                return;
            }

            EquipItemByUI(selectedItemInSlot, equipmentData);

        }

        private void EquipItemByUI(InventorySlot startSlot, EquipmentData equipmentData)
        {
            player.EquipItem(startSlot.ItemInstance, equipmentData);

            ItemInstance temporaryHolding = startSlot.ItemInstance;

            switch (equipmentData.EquipmentType)
            {
                case EquipmentType.Weapon:
                    {
                        selectedItemInSlot.SetItem(weaponSlot.ItemInstance);
                        weaponSlot.SetItem(temporaryHolding);
                        break;
                    }
                case EquipmentType.Armor:
                    {
                        selectedItemInSlot.SetItem(armorSlot.ItemInstance);
                        armorSlot.SetItem(temporaryHolding);
                        break;
                    }
                case EquipmentType.Accessory:
                    {
                        selectedItemInSlot.SetItem(accessorySlot.ItemInstance);
                        accessorySlot.SetItem(temporaryHolding);
                        break;
                    }
            }

            SFXManager.PlayClip(temporaryHolding.ItemData.UsageSoundClip, player.transform.position);
        }

        public void EquipItemBySave(ItemInstance equipmentInstance)
        {
            EquipmentData equipmentData = equipmentInstance.ItemData as EquipmentData;

            player.EquipItem(equipmentInstance, equipmentData);

            switch (equipmentData.EquipmentType)
            {
                case EquipmentType.Weapon:
                    {
                        weaponSlot.SetItem(equipmentInstance);
                        break;
                    }
                case EquipmentType.Armor:
                    {
                        armorSlot.SetItem(equipmentInstance);
                        break;
                    }
                case EquipmentType.Accessory:
                    {
                        accessorySlot.SetItem(equipmentInstance);
                        break;
                    }
                default:
                    break;
            }
        }

        public void OnDropButton()
        {
            if (selectedItemInSlot == null) return;

            if (selectedItemInSlot.ItemInstance.isEquipped)
                return;

            player.Inventory.Drop(selectedItemInSlot.ItemInstance);

            player.Inventory.RemoveItem(selectedItemInSlot.ItemInstance);
            selectedItemInSlot.ClearSlot(true);
            movingSlot.DropItem();

            UpdateButtons(null);
        }


        private void OnBeginInventorySlotDrag(InventorySlot draggedSlot)
        {
            if (selectedItemInSlot != null)
            {
                selectedItemInSlot.SetActiveOfSelectedFrame(false);
            }

            selectedItemInSlot = draggedSlot;
            movingSlot.CarryItem(draggedSlot);
            isDragging = true;
        }

        private void OnInventorySlotClick(InventorySlot clickedSlot, bool isSelectedBefore)
        {
            if (isDragging)
            {
                isDragging = false;
                movingSlot.DropItem();
                selectedItemInSlot?.SetActiveOfSelectedFrame(true);
                selectedItemInSlot = clickedSlot;
                UpdateButtons(clickedSlot.ItemInstance);
                return;
            }

            if (isSelectedBefore)
            {
                selectedItemInSlot = null;
                UpdateButtons(null);
                return;
            }

            selectedItemInSlot?.SetActiveOfSelectedFrame(false);

            selectedItemInSlot = clickedSlot;
            UpdateButtons(selectedItemInSlot.ItemInstance);
            return;
        }

        private void OnInventorySlotPointerDrop(InventorySlot targetSlot)
        {
            if (isDragging)
            {
                isDragging = false;

                if (selectedItemInSlot.SlotType == InventorySlotType.Inventory) // Inventory to Inventory
                {
                    selectedItemInSlot.SetItem(targetSlot.ItemInstance);
                    targetSlot.SetItem(movingSlot.ItemInstance);
                }
                else // Equipment to Inventory
                {
                    if (targetSlot.ItemInstance == null) // Inventory without Item
                    {
                        player.UnEquipItem(selectedItemInSlot.EquipmentType);
                        selectedItemInSlot.SetItem(null);
                        targetSlot.SetItem(movingSlot.ItemInstance);
                    }
                    else
                    {
                        EquipmentData equipmentData = targetSlot.ItemInstance.ItemData as EquipmentData;

                        // Inventory with correct equipment
                        if (equipmentData != null && equipmentData.EquipmentType == selectedItemInSlot.EquipmentType)
                        {
                            EquipItemByUI(targetSlot, equipmentData);
                        }
                        else
                        {
                            selectedItemInSlot.SetActiveItemOnly(true);
                        }
                    }


                }

                movingSlot.DropItem();
                selectedItemInSlot = null;
                UpdateButtons(null);
            }
        }

        private void OnEquipmentSlotPointerDrop(InventorySlot targetSlot)
        {
            if (!isDragging) return;

            isDragging = false;

            if (selectedItemInSlot.SlotType != InventorySlotType.Inventory)
            {
                movingSlot.DropItem();
                selectedItemInSlot.SetActiveItemOnly(true);
                selectedItemInSlot = null;
                UpdateButtons(null);
                return;
            }

            OnEquipButton();

            movingSlot.DropItem();
            selectedItemInSlot = null;
            UpdateButtons(null);

        }


        private void OnBackGroundDrop()
        {
            OnDropButton();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!isDragging) return;

            isDragging = false;
            movingSlot.DropItem();

            if (selectedItemInSlot.SlotType == InventorySlotType.Inventory)
            {
                selectedItemInSlot.SetActiveOfSelectedFrame(true);
                UpdateButtons(selectedItemInSlot.ItemInstance);
            }
            else
            {
                selectedItemInSlot.SetActiveItemOnly(true);
                selectedItemInSlot = null;
                UpdateButtons(null);
            }

        }
    }
}
