using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSP2
{

    public class EquipmentSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
    {
        [field: SerializeField] private EquipmentType slotType;
        protected InventoryUI inventoryUI;
        protected ItemInstance itemInstance;
        public ItemInstance ItemInstance { get => itemInstance; }

        [field: SerializeField] protected GameObject itemInSlot { get; set; }
        [field: SerializeField] protected Image itemImage { get; set; }

        public event Action<EquipmentSlot> DragBeginEvent;
        public event Action<EquipmentSlot> PointerDropEvent;

        public virtual void Initialize(InventoryUI _inventoryUI)
        {
            inventoryUI = _inventoryUI;
            if (itemInSlot == null)
            {
                Debug.Log("Item In Slot Not Assigned");
                itemInSlot = transform.GetChild(1).GetComponent<GameObject>();
            }
            if (itemImage == null)
            {
                Debug.Log("Item Image Not Assigned");
                itemImage = itemInSlot.transform.GetChild(1).GetComponent<Image>();
            }
            itemInSlot.SetActive(false);
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
        }


        public void ClearSlot(bool isRemoving)
        {
            itemInSlot.SetActive(false);
            if (isRemoving)
            {
                itemInstance = null;
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (itemInstance == null) return;

            ClearSlot(false);
            DragBeginEvent?.Invoke(this);
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
