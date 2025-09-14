using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RSP2
{
    public class InteractionNotice : MonoBehaviour
    {
        //[field: SerializeField] private TextMeshProUGUI buttonKey { get; set; }
        [field: SerializeField] private TextMeshProUGUI explanation { get; set; }

        public void ChangeString(string Interaction, string interactionTarget)
        {
            explanation.text = $"{interactionTarget} : {Interaction}";
        }

        
    }
}
