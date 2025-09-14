using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class InteractionHitBoxForPlayer : MonoBehaviour
    {
        private Player player;

        public void Initialize(Player _player)
        {
            player = _player;
        }

        private void OnTriggerEnter(Collider other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable?.OnInteractEnter(player);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable?.OnInteractExit(player);
            }
        }
    }
}
