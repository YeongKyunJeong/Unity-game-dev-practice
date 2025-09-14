using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class FixedUI : MonoBehaviour
    {
        private InGameManager gameManager;
        private CanvasUIManager canvasUIManager;

        [field: SerializeField] private PlayerInfoUI playerInfoUI;
        public PlayerInfoUI PlayerInfoUI { get => playerInfoUI; }

        public void Initialize(InGameManager _gameManager, CanvasUIManager _canvasUIManager)
        {
            gameManager = _gameManager;
            canvasUIManager = _canvasUIManager;

            if (playerInfoUI == null)
            {
                Debug.Log("Player Info UI Not Imported");
                playerInfoUI = GetComponent<PlayerInfoUI>();
            }
        }

    }
}
