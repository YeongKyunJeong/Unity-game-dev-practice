using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class AttackDataLibrary
    {
        [field: SerializeField] public AttackData BaseAttackData { get; private set; }
        [field: SerializeField] public List<AttackData> MeleeAttackDataList { get; private set; }
        [field: SerializeField] public List<AttackData> RangeAttackDataList { get; private set; }

        public AttackData GetMeleeAttackInfo(int index)
        {
            return MeleeAttackDataList.Count <= index ? null : MeleeAttackDataList[index];
        }

        public AttackData GetRangeAttackInfo(int index)
        {
            return RangeAttackDataList.Count <= index ? null : RangeAttackDataList[index];
        }
    }
}
