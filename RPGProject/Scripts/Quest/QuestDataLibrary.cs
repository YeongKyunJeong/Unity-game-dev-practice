using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class QuestDataLibrary : MonoBehaviour
    {
        [field: SerializeField] public List<QuestDataScriptableObject> QuestDataList { get; private set; }

        public QuestDataScriptableObject GetQuestDataInfo(int index)
        {
            return QuestDataList.Count <= index ? null : QuestDataList[index];
        }
    }

}
