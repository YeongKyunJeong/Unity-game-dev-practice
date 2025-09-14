using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class VFXDataLibrary
    {
        [field: SerializeField] public List<GameObject> HitVFX { get; private set; }

        [field: SerializeField] public List<GameObject> LevelUpVFX { get; private set; }

        // TODO :: Add other VFXs like lights
    }
}
