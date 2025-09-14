using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RSP2
{
    public class QuestProgress : MonoBehaviour
    {
        public QuestDataScriptableObject QuestDataSO;
        public QuestStatus QuestStatus;
        public List<ObjectiveProgress> ObjectiveProgresses;

        public QuestProgress(QuestDataScriptableObject newQuestDataSO)
        {
            ObjectiveProgresses = new List<ObjectiveProgress>();
            QuestDataSO = newQuestDataSO;
            QuestStatus = QuestStatus.Active;
            foreach (ObjectiveData objectiveData in newQuestDataSO.Objectives)
            {
                ObjectiveProgresses.Add(new ObjectiveProgress(objectiveData));
            }
        }

        public bool IsCompleted() => ObjectiveProgresses.All(x => x.IsComplete());
    }

}
