using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

namespace RSP2
{
    public enum QuestStatus
    {
        Inactive,
        Active,
        Completed,
        Failed
    }

    public class QuestManager : MonoSingleton<QuestManager>
    {
        private InGameManager gameManager;

        public List<QuestProgress> ActiveQuests = new List<QuestProgress>();

        [field: SerializeField] public QuestDataLibrary QuestDataLibrary { get; private set; }

        public void Initialize(InGameManager _gameManager)
        {
            gameManager = _gameManager;
        }

        private void Start()
        {
            if (gameManager == null) return;

            // TO DO :: Add Quest Data Loading Logic
        }

        public void StartQuest(QuestDataScriptableObject newQuestDataSO)
        {
            QuestProgress progress = new QuestProgress(newQuestDataSO);
            ActiveQuests.Add(progress);
            RegisterObjectives(progress);
        }

        private void RegisterObjectives(QuestProgress questProgress)
        {
            foreach (var obj in questProgress.ObjectiveProgresses)
            {
                switch (obj.ObjectiveData.Type)
                {
                    case ObjectiveType.CollectingItem:
                        {
                            EventBus.OnItemCollected += id => OnObjectiveEvent(obj, id, questProgress);
                        }
                        break;
                    case ObjectiveType.HuntingEnemy:
                        {
                            EventBus.OnEnemyHunted += id => OnObjectiveEvent(obj, id, questProgress);
                        }
                        break;
                    case ObjectiveType.TalkingToNPC:
                        {
                            EventBus.OnNPCTalked += id => OnObjectiveEvent(obj, id, questProgress);
                        }
                        break;
                    case ObjectiveType.ArrivingLocation:
                        {
                            EventBus.OnLocationArrived += id => OnObjectiveEvent(obj, id, questProgress);
                            break;
                        }
                }
            }
        }

        private void OnObjectiveEvent(ObjectiveProgress obj, int targetId, QuestProgress quest)
        {
            obj.OnEventTriggered(targetId);
            if (quest.IsCompleted())
            {
                quest.QuestStatus = QuestStatus.Completed;
                Debug.Log($"Äù½ºÆ® ¿Ï·á: {quest.QuestDataSO.QuestName}");
                EventBus.QuestCompleted?.Invoke(quest);
            }
        }

        public void GetQuestDataForSave(PlayerSaveData saveData)
        {
            List<QuestSaveData> questSaveDataList = new List<QuestSaveData>();

            foreach (QuestProgress questProgress in ActiveQuests)
            {
                QuestSaveData questSaveData = new QuestSaveData();

                questSaveData.QuestKey = questProgress.QuestDataSO.QuestKey;
                questSaveData.QuestStatus = questProgress.QuestStatus;

                List<int> currentCounts = new List<int>();
                foreach (ObjectiveProgress objectiveProgress in questProgress.ObjectiveProgresses)
                {
                    currentCounts.Add(objectiveProgress.CurrentCount);
                }
                questSaveData.CurrentCounts = currentCounts;
            }

            saveData.Quests = questSaveDataList;
        }

        public void SetQuestDataFromSave(PlayerSaveData saveData)
        {
            for (int i = 0; i < saveData.Quests.Count; i++)
            {
                QuestSaveData questSaveData = saveData.Quests[i];

                StartQuest(QuestDataLibrary.GetQuestDataInfo(questSaveData.QuestKey));
                ActiveQuests[i].QuestStatus = questSaveData.QuestStatus;
                for (int j = 0; j < questSaveData.CurrentCounts.Count; j++)
                {
                    ActiveQuests[i].ObjectiveProgresses[j].CurrentCount = questSaveData.CurrentCounts[j];
                }

            }

        }
    }
}
