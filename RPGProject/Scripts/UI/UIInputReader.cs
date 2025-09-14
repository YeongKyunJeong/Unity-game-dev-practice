using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class UIInputReader : MonoBehaviour
    {
        private InGameManager gameManager;
        private CanvasUIManager canvasUIManager;
        public event Action InventoryEvent;

        public void Initialize(InGameManager _gameManager)
        {
            gameManager = _gameManager;
            canvasUIManager = GetComponent<CanvasUIManager>();
        }

        public void OnInventory()
        {
            InventoryEvent?.Invoke();
        }

        public void OnClick()
        {
            // TO DO:: UI clicking logic
            Debug.Log("Click");
        }
    }
}
