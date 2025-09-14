using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RSP2
{
    public class PlayerInfoUI : MonoBehaviour
    {
        [field: SerializeField] private Slider hPSlide { get; set; }
        [field: SerializeField] private Slider mPSlide { get; set; }
        [field: SerializeField] private Slider staminaSlide { get; set; }

        public void UpdateHPUI(float value)
        {
            hPSlide.value = value;
        }

        public void UpdateMPUI(float value)
        {
            mPSlide.value = value;
        }

        public void UpdateStaminaUI(float value)
        {
            staminaSlide.value = value;
        }
    }
}


