using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "Custom/New Consumable")]
    public class ConsumableData : ItemData
    {
        [Header("Statistics Data")]
        public ConsumableEffect[] ConsumableEffects;
    }
}
