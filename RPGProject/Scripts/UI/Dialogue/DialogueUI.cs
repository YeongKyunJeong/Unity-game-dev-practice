using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace RSP2
{
    public class DialogueUI : MonoBehaviour
    {
        private InGameManager gameManager;

        [field: SerializeField] private DialogueDisplay nPCDialogueDisplay { get; set; }
        [field: SerializeField] private DialogueDisplay playerDialogueDisplay { get; set; }
        [field: SerializeField] private DialogueDisplay narratorDialogueDisplay { get; set; }
        [field: SerializeField] private DialogueDisplay currentDialogueDisplay;

        private DialogueData dialogueData { get; set; }
        private DialogueScript[] dialogueScriptSet { get; set; }
        private int scriptLength { get; set; }
        private int scriptIndex { get; set; }

        public void Initialize(InGameManager _gameManager)
        {
            gameManager = _gameManager;
            nPCDialogueDisplay.Initialize();
            playerDialogueDisplay.Initialize();
            narratorDialogueDisplay.Initialize();
            Deactivate();
        }

        public void StartDialogue(int key)
        {
            dialogueData = DataManager.Instance.TableDataLoader.
                DialogueDataLoader.GetByKey(key);
            dialogueScriptSet =
                 DataManager.Instance.TableDataLoader.
                 DialogueScriptsLoader.GetByMultipleKeys(dialogueData.ScriptKeys);

            scriptLength = dialogueScriptSet.Length;
            scriptIndex = 0;

            if (dialogueData.Random)
            {
                scriptIndex = Random.Range(0, scriptLength);
            }

            TalkOneScript(dialogueData, dialogueScriptSet[scriptIndex]);
        }

        public int Next()
        {
            if (currentDialogueDisplay.isPlaying)
            {
                currentDialogueDisplay.EndScript();
                return -1;
            }

            if (!dialogueData.Random && scriptIndex < scriptLength)
            {
                TalkOneScript(dialogueData, dialogueScriptSet[scriptIndex]);
                return -1;
            }

            return dialogueData.Redirection;
        }

        private void TalkOneScript(DialogueData data, DialogueScript script)
        {
            switch (script.Talker)
            {
                case 0: // Narrator
                    {
                        currentDialogueDisplay = narratorDialogueDisplay;
                        playerDialogueDisplay.Deactivate();
                        nPCDialogueDisplay.Deactivate();

                        narratorDialogueDisplay.Activate();
                        narratorDialogueDisplay.SetScript(script);
                        break;
                    }
                case 1: // Player
                    {
                        currentDialogueDisplay = playerDialogueDisplay;
                        narratorDialogueDisplay.Deactivate();
                        nPCDialogueDisplay.Deactivate();

                        playerDialogueDisplay.Activate();
                        playerDialogueDisplay.SetName(gameManager.Player.Name);
                        playerDialogueDisplay.SetScript(script);
                        break;
                    }
                case 2: // NPC
                    {
                        currentDialogueDisplay = nPCDialogueDisplay;
                        narratorDialogueDisplay.Deactivate();
                        playerDialogueDisplay.Deactivate();

                        nPCDialogueDisplay.Activate();
                        nPCDialogueDisplay.SetName(data.Name);
                        nPCDialogueDisplay.SetScript(script);
                        break;
                    }
            }

            scriptIndex++;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            currentDialogueDisplay?.Deactivate();
        }
    }
}
