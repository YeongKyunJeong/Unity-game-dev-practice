using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RSP2
{
    public enum PanelUIType
    {
        Inventory,
        Interaction,
        Dialogue,
        Menu
    }

    public class CanvasUIManager : MonoSingleton<CanvasUIManager>
    {
        private InGameManager inGameManager;
        private InteractionManager interactionManager;
        private Player player;
        private CombatSystemForPlayer combatSystem;
        private bool isOnDialogue;
        private bool isCutscene;
        public event Action cutsceneDialogueEndEvent;
        public event Action<int> interactionDialogueEndEvent;

        //[field: SerializeField] private UIInputReader uiInputReader;

        [field: SerializeField] public FixedUI FixedUI { get; private set; }
        [field: SerializeField] public PanelUI PanelUI { get; private set; }
        [field: SerializeField] public PopUpUI PopUpUI { get; private set; }

        public bool IsInventoryOpened { get; private set; }
        public bool IsMenuOpened { get; private set; }

        public void Initialize(InGameManager _gameManager)
        {
            inGameManager = _gameManager;
            player = _gameManager.Player;
            combatSystem = player.CombatSystem;

            player.InputReader.InventoryEvent += OpenInventoryUI;
            player.InputReader.MenuEvent += OpenMenuUI;

            if (FixedUI == null)
            {
                Debug.Log("Fixed UI Not Imported");
                FixedUI = GetComponent<FixedUI>();
            }
            FixedUI.Initialize(inGameManager, this);

            if (PanelUI == null)
            {
                Debug.Log("Panel UI Not Imported");
                PanelUI = GetComponent<PanelUI>();
            }
            PanelUI.Initialize(inGameManager, this);

            if (PopUpUI == null)
            {
                Debug.Log("Panel UI Not Imported");
                PopUpUI = GetComponent<PopUpUI>();
            }
            PopUpUI.Initialize(inGameManager, this);

            IsInventoryOpened = false;

        }

        private void Start()
        {
            inGameManager.Player.InputReader.ClickWhileInteractionEvent += OnNextInput;

            player.CombatSystem.DamageEvent += ChangeHPBar;
            player.CombatSystem.HealEvent += ChangeHPBar;
            player.CombatSystem.MPRecoveryEvent += ChangeMPBar;
            player.CombatSystem.MPSpendEvent += ChangeMPBar;
            player.CombatSystem.StaminaRecoveryEvent += ChangeStaminaBar;
            player.CombatSystem.StaminaSpendEvent += ChangeStaminaBar;
        }

        public void SetPanelUIActive(PanelUIType uIType, bool isOn = true, int count = 1)
        {
            switch (uIType)
            {
                case PanelUIType.Inventory:
                    {
                        PanelUI.InventoryUI.Open();

                        //bool isActive = PanelUI.InventoryUI.IsActive;
                        IsInventoryOpened = PanelUI.InventoryUI.IsActive;
                        inGameManager.OnPanelUIOpen(IsInventoryOpened);
                        break;
                    }
                case PanelUIType.Interaction:
                    {
                        if (isOn) PanelUI.InteractionUI.Activate(count);
                        else PanelUI.InteractionUI.Deactivate(count);
                        break;
                    }
                case PanelUIType.Dialogue:
                    {
                        if (isOn) PanelUI.DialogueUI.Activate();
                        else PanelUI.DialogueUI.Deactivate();
                        break;
                    }
                case PanelUIType.Menu:
                    {
                        PanelUI.MenuUI.Open();

                        IsMenuOpened = PanelUI.MenuUI.IsActive;
                        if (isOn) PanelUI.DialogueUI.Activate();
                        else PanelUI.DialogueUI.Deactivate();
                        inGameManager.OnPanelUIOpen(IsMenuOpened);
                        break;

                    }
            }

        }

        #region Fixed UI Methods

        public void ChangeHPBar(float leftHP, float maxHP)
        {
            FixedUI.PlayerInfoUI.UpdateHPUI(leftHP / maxHP);
        }

        public void ChangeMPBar()
        {
            FixedUI.PlayerInfoUI.UpdateMPUI(player.CombatSystem.CurrentMP / player.CombatSystem.MaxMP);
        }

        public void ChangeStaminaBar()
        {
            FixedUI.PlayerInfoUI.UpdateStaminaUI(player.CombatSystem.CurrentStamina / player.CombatSystem.MaxStamina);
        }
        #endregion

        #region Panel UI Methods

        #region Dialogue
        public void SendDialogueStartCall(int key, bool _isCutscene = false)
        {
            if (_isCutscene)
            {
                isCutscene = _isCutscene;
                PanelUI.SetCanvasSortOrder(10001);
            }

            isOnDialogue = true;
            PanelUI.DialogueUI.StartDialogue(key);
        }

        public void SendInteractionUITMPChangeCall(string targetName, string interactionName, int count)
        {
            PanelUI.InteractionUI.ChangeInteractionDisplayTMP(targetName, interactionName, count);
        }

        public void OnNextInput()
        {
            if (isOnDialogue)
            {
                int next = PanelUI.DialogueUI.Next();
                if (next >= 0) // Means this Dialogue Ends
                {
                    isOnDialogue = false;
                    // If it is Cutscene, No Need to Invoke
                    if (isCutscene)
                    {
                        isCutscene = false;
                        cutsceneDialogueEndEvent?.Invoke();
                    }
                    else
                    {
                        PanelUI.DialogueUI.Deactivate();
                        interactionDialogueEndEvent?.Invoke(next);
                    }
                }
            }
            else
            {
                PanelUI.SetCanvasSortOrder();
                PanelUI.DialogueUI.Deactivate();
                cutsceneDialogueEndEvent?.Invoke();
            }
        }
        #endregion

        private void OpenInventoryUI()
        {
            // TO DO :: Check Other UI is Opened
            if (PanelUI.MenuUI.IsActive) return;

            // Open Inventory Only When Menu UI is Not Opened
            SetPanelUIActive(PanelUIType.Inventory);

        }
        #endregion

        private void OpenMenuUI()
        {
            // TO DO :: Check Other UI is Opened
            if (PanelUI.InventoryUI.IsActive)
            {
                SetPanelUIActive(PanelUIType.Inventory);
                return;
            }

            SetPanelUIActive(PanelUIType.Menu);
        }

        #region Pop Up UI Methods

        #endregion
    }
}
