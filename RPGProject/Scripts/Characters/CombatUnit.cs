using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [RequireComponent(typeof(CombatSystem))]
    public abstract class CombatUnit : MonoBehaviour
    {

        protected abstract void OnDie();

    }
}
