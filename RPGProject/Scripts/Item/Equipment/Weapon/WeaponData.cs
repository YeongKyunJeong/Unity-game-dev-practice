using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [CreateAssetMenu(fileName = "Item", menuName = "Custom/New Weapon")]
    public class WeaponData : EquipmentData
    {
        [field: SerializeField] public GameObject EquipPrefab { get; private set; }
        [field: SerializeField] public AudioClip AttackSoundClip { get; private set; }

        [field: SerializeField] public DamageType DamageType { get; private set; }
        [field: SerializeField] public float RangeModifier { get; private set; }
        [field: SerializeField] public float SpeedModifier { get; private set; }
        [field: SerializeField] public float[] DamageBonus { get; private set; }
        [field: SerializeField] public float IntensityBonus { get; private set; }
    }
}
