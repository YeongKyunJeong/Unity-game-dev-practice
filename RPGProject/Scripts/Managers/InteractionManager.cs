using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RSP2
{
    public class InteractionManager : MonoSingleton<InteractionManager>
    {
        private InGameManager inGameManager;
        private CameraManager cameraManager;
        private CanvasUIManager canvasUIManager;
        private Player player;
        private ActionStateMachineForPlayer playerStateMachine;

        [field: SerializeField] private InteractionStringTable InteractionStringSO { get; set; }

        [field: SerializeField] private InteractionUI interactionUI;
        [field: SerializeField] private InteractionNotice interactionDisplay;

        //private Dictionary<NPC, NPCInteraction> NPCInteractions;
        [field: SerializeField] private bool isInteracting;

        [field: SerializeField] private List<KeyValuePair<NPC, NPCInteraction>> interactionPairList;
        private int currentIndex;
        [field: SerializeField] private KeyValuePair<NPC, NPCInteraction> currentPair;

        public InteractionUI InteractionUI
        {
            get
            {
                if (interactionUI == null)
                {
                    interactionUI = FindObjectOfType<InteractionUI>();
                    if (!interactionUI)
                    {
                        Debug.LogError("Interaction UI Not Detected");
                        return null;
                    }
                }

                return interactionUI;
            }
        }


        public void Initialize(InGameManager _gameManager, CameraManager _cameraManager, CanvasUIManager _canvasUIManager)
        {
            inGameManager = _gameManager;
            cameraManager = _cameraManager;
            canvasUIManager = _canvasUIManager;
            //NPCInteractions = new Dictionary<NPC, NPCInteraction>();
            canvasUIManager.interactionDialogueEndEvent += OnDialogueEnd;
            interactionPairList = new List<KeyValuePair<NPC, NPCInteraction>>();
            isInteracting = false;
            currentIndex = 0;
        }

        private void Start()
        {
            //if (inGameManager == null) return;

            player = inGameManager.Player;
            playerStateMachine = player.ActionStateMachine;
            player.InputReader.InteractionEvent += OnInteractionInput;
            player.InputReader.InteractionChangeEvent += OnNextInteractionInput;
            //isInteractable = CheckIsInteractable();
        }

        private bool CheckIsInteractable()
        {
            // TO DO :: Add checking logic whether is interactable
            if (!playerStateMachine.isOnLand) return false;

            if (canvasUIManager.IsInventoryOpened) return false;

            if (playerStateMachine.isDead) return false;

            return true;
        }

        public void AddNPCInteraction(NPC nPC, NPCInteraction newNPCInteraction)
        {
            KeyValuePair<NPC, NPCInteraction> newPair = new KeyValuePair<NPC, NPCInteraction>(nPC, newNPCInteraction);
            interactionPairList.Add(newPair);
            currentIndex = interactionPairList.Count - 1;
            canvasUIManager.SetPanelUIActive(PanelUIType.Interaction, true, currentIndex + 1);
            ChangeCurrentInteraction(newPair);
        }

        public void RemoveNPCInteraction(NPC nPC)
        {
            interactionPairList.RemoveAll(kvp => kvp.Key == nPC);

            if (interactionPairList.Count == 0)
            {
                canvasUIManager.SetPanelUIActive(PanelUIType.Interaction, false, 0);
                currentPair = new KeyValuePair<NPC, NPCInteraction>();
            }
            else
            {
                if (interactionPairList.Contains(currentPair)) return;

                currentIndex = interactionPairList.Count - 1;
                ChangeCurrentInteraction(interactionPairList[currentIndex]);
            }
        }

        public void ChangeCurrentInteraction(KeyValuePair<NPC, NPCInteraction> nextPair)
        {
            currentPair = nextPair;
            canvasUIManager.SendInteractionUITMPChangeCall(nextPair.Key.Name, GetInteractionName(nextPair.Value), interactionPairList.Count);
        }

        public string GetInteractionName(NPCInteraction nPCInteraction)
        {
            return InteractionStringSO.GetString(nPCInteraction);
        }

        private void OnInteractionInput()
        {
            if (isInteracting) return;

            if (!CheckIsInteractable()) return;

            if (currentPair.Key == null) return;

            StartInteraction();
        }

        public void OnNextInteractionInput()
        {
            int totalCount = interactionPairList.Count;
            if (totalCount <= 1)
            {
                return;
            }
            currentIndex++;
            currentIndex = currentIndex < totalCount ? currentIndex : 0;
            ChangeCurrentInteraction(interactionPairList[currentIndex]);
        }

        private void StartInteraction()
        {
            isInteracting = true;
            canvasUIManager.SetPanelUIActive(PanelUIType.Interaction, false);
            canvasUIManager.SetPanelUIActive(PanelUIType.Dialogue, true);
            cameraManager.CallCameraSwitching(currentPair.Key.NPCCamera.VirtualCamera);
            inGameManager.OnInteractionUIOpen(true);

            switch (currentPair.Value)
            {
                case NPCInteraction.Speakable:
                    canvasUIManager.SendDialogueStartCall(currentPair.Key.DialogueKey);
                    break;
                case NPCInteraction.Tradable:

                    break;
                default:
                    break;
            }
        }

        private void EndCurrentInteraction()
        {
            switch (currentPair.Value)
            {
                case NPCInteraction.Speakable:
                    {
                        canvasUIManager.SetPanelUIActive(PanelUIType.Interaction, true);
                        canvasUIManager.SetPanelUIActive(PanelUIType.Dialogue, false);
                        cameraManager.CallCameraSwitching(null);
                        inGameManager.OnInteractionUIOpen(false);

                        break;
                    }
            }

            isInteracting = false;
        }

        private void OnDialogueEnd(int redirection)
        {
            currentPair.Key.DialogueKey = redirection;
            EndCurrentInteraction();
        }
    }
}
