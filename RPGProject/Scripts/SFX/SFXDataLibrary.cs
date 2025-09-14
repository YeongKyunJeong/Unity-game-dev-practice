using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class SFXDataLibrary
    {
        [field: SerializeField] public List<AudioClip> SlashingHitSounds { get; private set; }

        [field: SerializeField] public List<AudioClip> BlungingHitSounds { get; private set; }
    }
}
