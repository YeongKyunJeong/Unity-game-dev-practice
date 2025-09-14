using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [CreateAssetMenu(fileName = "SFXSO", menuName = "Custom/SO/SFX")]
    public class SFXScriptableObject : ScriptableObject
    {
        [field: SerializeField] public SFXDataLibrary SFXDataLibrary {get; private set;}
    }
}
