using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RSP2
{
    public class MovingSlot : MonoBehaviour
    {
        private InventoryUI inventoryUI;
        private ItemInstance itemInstance;
        public ItemInstance ItemInstance { get { return itemInstance; } }

        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI amountTMP;

        private bool isMoving;


        public void Initialize(InventoryUI _inventoryUI)
        {
            inventoryUI = _inventoryUI;
            if (itemImage == null)
            {
                Debug.Log("Item Image Not Assigned");
                itemImage = transform.GetChild(1).GetComponent<Image>();
            }
            if (amountTMP == null)
            {
                Debug.Log("Amount TMP Not Assigned");
                amountTMP = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            }
            gameObject.SetActive(false);
            isMoving = false;
        }

        private void Update()
        {
            if (isMoving)
            {
                transform.position = Mouse.current.position.ReadValue();
            }
        }

        public void CarryItem(InventorySlot DraggedItemSlot)
        {
            transform.position = Mouse.current.position.ReadValue();
            itemInstance = DraggedItemSlot.ItemInstance;
            itemImage.sprite = itemInstance.ItemData.ItemSprite;

            if (itemInstance.Amount <= 1)
            {
                amountTMP.gameObject.SetActive(false);
            }
            else
            {
                amountTMP.gameObject.SetActive(true);
            }
            amountTMP.text = itemInstance.Amount.ToString();
            isMoving = true;
            gameObject.SetActive(true);
        }

        public void CarryItem(EquipmentSlot DraggedSlot)
        {
            transform.position = Mouse.current.position.ReadValue();
            itemInstance = DraggedSlot.ItemInstance;
            itemImage.sprite = itemInstance.ItemData.ItemSprite;


            amountTMP.gameObject.SetActive(false);

            amountTMP.text = itemInstance.Amount.ToString();
            isMoving = true;
            gameObject.SetActive(true);
        }

        public ItemInstance DropItem()
        {
            gameObject.SetActive(false);
            itemInstance = null;
            isMoving = false;
            return itemInstance;
        }
    }
}
