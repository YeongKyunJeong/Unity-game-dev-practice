using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [CreateAssetMenu(fileName = "QuestData", menuName = "Custom/New Quest Data")]
    public class QuestDataScriptableObject :ScriptableObject
    {
        public int QuestKey;
        public string QuestName;
        public string Description;
        public List<ObjectiveData> Objectives;
    }
}
