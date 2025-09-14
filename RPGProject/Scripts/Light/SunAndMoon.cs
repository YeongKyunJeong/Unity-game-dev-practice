using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class SunAndMoon : MonoBehaviour
    {
        [field: SerializeField] public Light Sun { get; private set; }
        [field: SerializeField] public Light Moon { get; private set; }
    }
}
