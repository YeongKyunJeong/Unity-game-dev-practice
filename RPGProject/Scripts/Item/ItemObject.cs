using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public interface IInteractable
    {
        public string GetInteractMsg();
        public void OnInteractEnter(Player player);
        public void OnInteractExit(Player player);
    }



    public class ItemObject : MonoBehaviour, IInteractable
    {
        public ItemData itemData;
        public int Upgrade = 0;
        public int Amount = 1;

        public string GetInteractMsg()
        {
            if (itemData == null)
                return "Pickup Unknown";
            else
                return string.Format("Pickup {0} {1}", itemData.DisplayName, Amount);
        }

        public void OnInteractEnter(Player player)
        {
            if (player.AddItem(itemData, Amount, Upgrade))
                Destroy(gameObject);
        }

        public void OnInteractExit(Player player)
        {
            // TO DO :: Exit logic
        }
    }
}
