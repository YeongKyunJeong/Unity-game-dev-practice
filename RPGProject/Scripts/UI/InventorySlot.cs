using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSP2
{
    public enum InventorySlotType
    {
        Inventory,
        Equipment
    }

    public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IDropHandler /*IPointerUpHandler*/
    {
        private InventoryUI inventoryUI;
        private ItemInstance itemInstance;
        public ItemInstance ItemInstance { get => itemInstance; }

        [field: SerializeField] public InventorySlotType SlotType { get; private set; }
        [field: SerializeField] public EquipmentType EquipmentType { get; private set; }

        [field: SerializeField] private GameObject itemInSlot { get; set; }
        [field: SerializeField] private Image itemImage { get; set; }
        [field: SerializeField] private TextMeshProUGUI amountTMP { get; set; }
        [field: SerializeField] private GameObject selectedFrame { get; set; }

        public event Action<InventorySlot, bool> ClickEvent;
        public event Action<InventorySlot> DragBeginEvent;
        public event Action<InventorySlot> PointerDropEvent;

        public void Initialize(InventoryUI _inventoryUI)
        {
            inventoryUI = _inventoryUI;
            if (itemInSlot == null)
            {
                Debug.Log("Item In Slot Not Assigned");
                itemInSlot = transform.GetChild(2).GetComponent<GameObject>();
            }
            if (itemImage == null)
            {
                Debug.Log("Item Image Not Assigned");
                itemImage = itemInSlot.transform.GetChild(1).GetComponent<Image>();
            }
            if (amountTMP == null)
            {
                Debug.Log("Amount TMP Not Assigned");
                amountTMP = itemInSlot.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            }
            if (selectedFrame == null)
            {
                Debug.Log("Selected Not Assigned");
                selectedFrame = transform.GetChild(1).GetComponent<GameObject>();
            }

            itemInSlot.SetActive(false);
            selectedFrame.SetActive(false);

            if (SlotType != InventorySlotType.Inventory)
            {
                amountTMP.text = string.Empty;
            }
        }

        public void SetItem(ItemInstance newItem)
        {
            if (newItem == null)
            {
                ClearSlot(true);
                return;
            }

            itemInSlot.SetActive(true);
            itemInstance = newItem;
            itemImage.sprite = itemInstance.ItemData.ItemSprite;

            if (SlotType == InventorySlotType.Inventory)
            {
                AmountTMPChange(newItem.Amount);
                amountTMP.text = newItem.Amount.ToString();
                itemInstance.AmountChangeEvent += AmountTMPChange;
            }
        }

        public void ClearSlot(bool isRemoving)
        {
            itemInSlot.SetActive(false);



            if (isRemoving)
            {
                selectedFrame.SetActive(false);

                if (SlotType == InventorySlotType.Inventory)
                {
                    amountTMP.text = string.Empty;
                    itemInstance.AmountChangeEvent -= AmountTMPChange;
                }

                itemInstance = null;
                return;

            }
        }

        public void AmountTMPChange(int changedAmount)
        {
            if (changedAmount <= 1)
            {
                amountTMP.gameObject.SetActive(false);
                return;
            }
            amountTMP.gameObject.SetActive(true);
            amountTMP.text = changedAmount.ToString();
        }

        public void CloseInventory()
        {
            selectedFrame.SetActive(false);
            // TO DO :: Add logic called when inventory closed
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (SlotType != InventorySlotType.Inventory) return;

            if (itemInstance == null) return;

            bool isSelectedBefore = selectedFrame.activeSelf;
            ClickEvent?.Invoke(this, isSelectedBefore);
            selectedFrame.SetActive(!isSelectedBefore);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (itemInstance == null) return;

            ClearSlot(false);
            DragBeginEvent?.Invoke(this);
        }

        public void SetActiveOfSelectedFrame(bool isOn)
        {
            selectedFrame.SetActive(isOn);
            if (isOn)
            {
                itemInSlot.SetActive(true);
            }
        }

        public void SetActiveItemOnly(bool isOn)
        {
            itemInSlot.SetActive(isOn);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // To use DragBegin
        }

        public void OnDrop(PointerEventData eventData)
        {
            PointerDropEvent?.Invoke(this);
        }
    }
}
