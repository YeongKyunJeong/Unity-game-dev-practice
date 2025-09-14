using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public enum ObjectiveType
    {
        CollectingItem,
        HuntingEnemy,
        TalkingToNPC,
        ArrivingLocation,
        AccessUI
    }

    [System.Serializable]
    public class ObjectiveData
    {
        public ObjectiveType Type;
        public int TargetKey;
        public int RequiredCount;
    }

    [System.Serializable]
    public class ObjectiveProgress
    {
        public ObjectiveData ObjectiveData;
        public int CurrentCount;
        
        public ObjectiveProgress(ObjectiveData newObjectiveData)
        {
            ObjectiveData = newObjectiveData;
            CurrentCount = 0;
        }

        public bool IsComplete() => CurrentCount >= ObjectiveData.RequiredCount;
        
        public void OnEventTriggered(int targetKey)
        {
            if (ObjectiveData.TargetKey != targetKey) return;
            if (IsComplete()) return;
            CurrentCount++; 
        } 
    }
}
