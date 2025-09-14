using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [CreateAssetMenu(fileName = "VFXSO", menuName = "Custom/SO/VFX")]
    public class VFXScriptableObject : ScriptableObject
    {
        [field: SerializeField] public VFXDataLibrary VFXDataLibrary { get; private set; }

    }
}
