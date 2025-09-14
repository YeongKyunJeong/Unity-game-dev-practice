using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace RSP2
{
    public class InteractionUI : MonoBehaviour
    {
        private InGameManager gameManager;
        private CanvasUIManager canvasUIManager;

        [field: SerializeField] private InteractionNotice interactionNotice;
        [field: SerializeField] private GameObject nextNotice;

        public void Initialize(InGameManager _gameManager, CanvasUIManager _canvasUIManager)
        {

            if (interactionNotice == null)
            {
                Debug.LogError("Interaction Display Not Assigned");
            }

            gameManager = _gameManager;
            canvasUIManager = _canvasUIManager;
            Deactivate(0);
        }

        public void Activate(int count)
        {
            gameObject.SetActive(true);
            SetNextNoticeActive(count);
        }

        public void Deactivate(int count)
        {
            gameObject.SetActive(false);
            SetNextNoticeActive(count);
        }

        private void SetNextNoticeActive(int count)
        {
            if (count > 1) nextNotice.SetActive(true);
            else nextNotice.SetActive(false);
        }


        public void ChangeInteractionDisplayTMP(NPC nPC, NPCInteraction nPCInteraction, int count)
        {
            Activate(count);
            string InteractionString = InteractionManager.Instance.GetInteractionName(nPCInteraction);
            interactionNotice.ChangeString(InteractionString, nPC.Name);
        }

        public void ChangeInteractionDisplayTMP(string targetName, string interactionName, int count)
        {
            Activate(count);
            interactionNotice.ChangeString(interactionName, targetName);
        }
    }
}
