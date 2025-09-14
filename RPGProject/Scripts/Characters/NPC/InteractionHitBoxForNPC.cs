using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RSP2
{
    [Flags]
    public enum NPCInteraction
    {
        Speakable = 1,
        Tradable = 2,
    }

    public class InteractionHitBoxForNPC : MonoBehaviour, IInteractable
    {
        [field: SerializeField] private NPCInteraction interaction;
        [field: SerializeField] private NPC myNPC;

        public void Initialize(NPC NPC, bool[] initInteractions)
        {
            if (NPC == null)
            {
                NPC = transform.parent.GetComponent<NPC>();
            }
            else
            {
                myNPC = NPC;
            }

            interaction = 0;

            int count = Enum.GetValues(typeof(NPCInteraction)).Length;
            NPCInteraction[] newInteractions = (NPCInteraction[])Enum.GetValues(typeof(NPCInteraction));

            for (int i = 0; i < initInteractions.Length; i++)
            {
                if (i < count && initInteractions[i])
                {
                    interaction |= newInteractions[i];
                }
            }
        }

        public void AddInteraction(NPCInteraction newInteraction)
        {
            interaction |= newInteraction;
        }

        public void RemoveInteraction(NPCInteraction removedInteraction)
        {
            interaction &= ~removedInteraction;
        }

        public string GetInteractMsg()
        {
            return string.Empty;
        }

        public void OnInteractEnter(Player player)
        {
            NPCInteraction[] newInteractions = (NPCInteraction[])Enum.GetValues(typeof(NPCInteraction));
            foreach (var item in newInteractions)
            {
                if (interaction.HasFlag(item))
                {
                    InteractionManager.Instance.AddNPCInteraction(myNPC, item);
                }
            }
        }

        public void OnInteractExit(Player player)
        {
            InteractionManager.Instance.RemoveNPCInteraction(myNPC);
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    Player _player = other.GetComponent<Player>();
        //    if (_player != null)
        //    {

        //        OnInteractEnter(_player);
        //    }
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    Player _player = other.GetComponent<Player>();
        //    if (_player != null)
        //    {
        //        OnInteractExit(_player);
        //    }
        //}
    }
}
