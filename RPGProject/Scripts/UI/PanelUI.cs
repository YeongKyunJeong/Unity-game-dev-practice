using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RSP2
{
    public class PanelUI : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        private InGameManager inGameManager;
        private CanvasUIManager canvasUIManager;

        [field: SerializeField] private Canvas canvas;
        [field: SerializeField] private EquipmentStatsDisplay statsDisplay;
        public EquipmentStatsDisplay StatsDisplay { get => statsDisplay; }

        [field: SerializeField] private InventoryUI inventoryUI;
        public InventoryUI InventoryUI { get => inventoryUI; }

        [field: SerializeField] private InteractionUI interactionUI;
        public InteractionUI InteractionUI { get => interactionUI; }

        [field: SerializeField] private DialogueUI dialogueUI;
        public DialogueUI DialogueUI { get => dialogueUI; }

        [field: SerializeField] private MenuUI menuUI;
        public MenuUI MenuUI { get => menuUI; }
        public event Action PointerDropEvent;

        public void Initialize(InGameManager _inGameManager, CanvasUIManager _canvasUIManager)
        {
            inGameManager = _inGameManager;
            canvasUIManager = _canvasUIManager;

            if (canvas == null)
            {
                Debug.Log("Canvas Not Imported");
                canvas = GetComponentInChildren<Canvas>();
            }
            if (statsDisplay == null)
            {
                Debug.Log("Stats Display Not Imported");
                statsDisplay = GetComponentInChildren<EquipmentStatsDisplay>();
            }
            if (inventoryUI == null)
            {
                Debug.Log("Inventory UI Not Imported");
                inventoryUI = GetComponentInChildren<InventoryUI>();
            }
            if (interactionUI == null)
            {
                Debug.Log("interactionUI UI Not Imported");
                interactionUI = GetComponentInChildren<InteractionUI>();
            }
            if (dialogueUI == null)
            {
                Debug.Log("dialogueUI UI Not Imported");
                dialogueUI = GetComponentInChildren<DialogueUI>();
            }
            if (menuUI == null)
            {
                Debug.Log("menuUI UI Not Imported");
                menuUI = GetComponentInChildren<MenuUI>();
            }

            statsDisplay.Initialize(inGameManager, canvasUIManager);
            inventoryUI.Initialize(inGameManager, canvasUIManager, this);
            interactionUI.Initialize(inGameManager, canvasUIManager);
            dialogueUI.Initialize(inGameManager);
            menuUI.Initialize(inGameManager);
        }

        public void OnDrop(PointerEventData eventData)
        {
            PointerDropEvent?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("click");
        }

        public void SetCanvasSortOrder(int newSortOrder = 0)
        {
            canvas.sortingOrder = newSortOrder;
        }
    }
}
