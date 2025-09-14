using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class EventBus : MonoBehaviour
    {
        public static Action<int> OnItemCollected;
        public static Action<int> OnEnemyHunted;
        public static Action<int> OnNPCTalked;
        public static Action<string> OnUIAccessed;
        public static Action<int> OnLocationArrived;
        public static Action<QuestProgress> QuestCompleted;

        public static void TriggerItemCollected(int id)
            => OnItemCollected?.Invoke(id);
        public static void TriggerEnemyHunted(int id) => OnEnemyHunted?.Invoke(id);
        public static void TriggerNPCTalked(int id) => OnNPCTalked?.Invoke(id);
        public static void TriggerLocationArrived(int id) => OnLocationArrived?.Invoke(id);
        public static void TriggerUIAccessed(string name) => OnUIAccessed?.Invoke(name);
    }
}

