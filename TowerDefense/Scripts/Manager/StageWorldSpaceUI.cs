using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{

    public class StageWorldSpaceUI : MonoBehaviour
    {
        [SerializeField] private StageUITMPSetter stageUITMPSetter;

        public void Initialize()
        { 
                return;

        }

        public void ChangeValue(StageUITMPType tagetTMP, float targetValue)
        {
            stageUITMPSetter.ChangeValue(tagetTMP, targetValue);
        }

        public void ChangeValue(StageUITMPType tagetTMP, int targetValue)
        {
            stageUITMPSetter.ChangeValue(tagetTMP, targetValue);
        }
    }
}
