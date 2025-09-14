using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class PopUpUI : MonoBehaviour
    {
        private InGameManager gameManager;
        private CanvasUIManager canvasUIManager;
        public void Initialize(InGameManager _gameManager, CanvasUIManager _canvasUIManager)
        {
            gameManager = _gameManager;
            canvasUIManager = _canvasUIManager;
        }
    }
}
