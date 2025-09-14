using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RSP2
{
    public class EquipmentStatsDisplay : MonoBehaviour
    {
        private InGameManager gameManager;
        private CanvasUIManager canvasUIManager;
        private Player player;

        [field: SerializeField] private TextMeshProUGUI LevelTMP;

        [field: SerializeField] private TextMeshProUGUI CurrentExpTMP;
        [field: SerializeField] private TextMeshProUGUI NextExpTMP;

        [field: SerializeField] private TextMeshProUGUI MaxHPTMP;
        [field: SerializeField] private TextMeshProUGUI MaxMPTMP;
        [field: SerializeField] private TextMeshProUGUI MaxStaminaTMP;

        [field: SerializeField] private TextMeshProUGUI CurrentHPTMP;
        [field: SerializeField] private TextMeshProUGUI CurrentMPTMP;
        [field: SerializeField] private TextMeshProUGUI CurrentStaminaTMP;

        [field: SerializeField] private TextMeshProUGUI BaseAttackTMP;
        [field: SerializeField] private TextMeshProUGUI WeaponAttackTMP;
        [field: SerializeField] private TextMeshProUGUI AttackSpeedTMP;
        [field: SerializeField] private TextMeshProUGUI BaseDefenseTMP;
        [field: SerializeField] private TextMeshProUGUI ArmorDefenseTMP;
        [field: SerializeField] private TextMeshProUGUI MovementSpeedTMP;

        public void Initialize(InGameManager _gameManager, CanvasUIManager _canvasUIManager)
        {
            gameManager = _gameManager;
            canvasUIManager = _canvasUIManager;

            player = gameManager.Player;

            player.EquipmentChangeEvent += OnEquipmentChange;
            player.UnequipmentEvent += OnUnEquip;

            player.StatHandler.LevelChangeEvent += OnLevelChange;
            player.StatHandler.BaseStatChangeEvent += OnBaseStatsChange;
        }

        public void OnBaseStatsChange(StatsToDisplay statsToDisplay, float newValue)
        {
            switch (statsToDisplay)
            {

                case StatsToDisplay.CurrentExp:
                    {
                        CurrentExpTMP.text = Mathf.Round(newValue).ToString();
                        break;
                    }
                case StatsToDisplay.CurrentHP:
                    {
                        CurrentHPTMP.text = Mathf.Round(newValue).ToString();
                        break;
                    }
                case StatsToDisplay.CurrentMP:
                    {
                        CurrentMPTMP.text = Mathf.Round(newValue).ToString();
                        break;
                    }
                case StatsToDisplay.CurrentStamina:
                    {
                        CurrentStaminaTMP.text = Mathf.Round(newValue).ToString();
                        break;
                    }
            }
        }


        public void OnLevelChange(int newLevel, int newExp, int currentExp, StatForPlayer statData, CombatSystem combatSystem, bool isAllSetting)
        {
            LevelTMP.text = newLevel.ToString();

            NextExpTMP.text = newExp.ToString();
            CurrentExpTMP.text = currentExp.ToString();

            MaxHPTMP.text = Mathf.Round(statData.MaxHP).ToString();
            MaxMPTMP.text = Mathf.Round(statData.MaxMP).ToString();
            MaxStaminaTMP.text = Mathf.Round(statData.MaxStamina).ToString();

            CurrentHPTMP.text = Mathf.Round(combatSystem.CurrentHP).ToString();
            CurrentMPTMP.text = Mathf.Round(combatSystem.CurrentMP).ToString();
            CurrentStaminaTMP.text = Mathf.Round(combatSystem.CurrentStamina).ToString();

            BaseAttackTMP.text = (Mathf.Round(statData.Attack * 10) / 10).ToString("0.0");
            BaseDefenseTMP.text = (Mathf.Round(statData.Defense * 10) / 10).ToString("0.0");

            if (isAllSetting)
            {
                if (player.CurrentWeapon != null)
                {
                    WeaponAttackTMP.text = player.CurrentWeapon.WeaponData.DamageBonus[player.CurrentWeapon.ItemInstance.Upgrade].ToString("0.0");
                    AttackSpeedTMP.text =
                        (Mathf.Round(player.CurrentWeapon.WeaponData.SpeedModifier * statData.AttackSpeed * 10) / 10).ToString("0.0");
                }
                else
                {
                    WeaponAttackTMP.text = "0.0";
                    AttackSpeedTMP.text = (Mathf.Round(statData.AttackSpeed * 10) / 10).ToString("0.0");
                }

                // TO DO:: Add logic for armor and accessory
                ArmorDefenseTMP.text = "0.0";
                MovementSpeedTMP.text = (Mathf.Round(statData.MovementSpeed * 10) / 10).ToString("0.0");
            }


        }

        public void OnEquipmentChange(ItemInstance equipment, StatForPlayer statData)
        {
            EquipmentData equipmentData = equipment.ItemData as EquipmentData;

            if (equipmentData == null) return; // Should Not be Called

            switch (equipmentData)
            {
                case WeaponData weaponData:
                    {
                        WeaponAttackTMP.text = (Mathf.Round(weaponData.DamageBonus[equipment.Upgrade] * 10) / 10).ToString("0.0");
                        AttackSpeedTMP.text =
                            (Mathf.Round(weaponData.SpeedModifier * statData.AttackSpeed * 10) / 10).ToString("0.0");
                        // TO DO :: Add other special effect logic
                        break;
                    }
                    // TO DO :: Add other equipment logic
            }
        }

        //public void OnEquipmentChange<T>(T equipmentData, StatForPlayer statData) where T : EquipmentData
        //{
        //    switch (equipmentData)
        //    {
        //        case WeaponData weaponData:
        //            {
        //                WeaponAttackTMP.text = (Mathf.Round(weaponData.DamageBonus * 10) / 10).ToString("0.0");
        //                AttackSpeedTMP.text =
        //                    (Mathf.Round(weaponData.SpeedModifier * statData.AttackSpeed * 10) / 10).ToString("0.0");
        //                // TO DO :: Add other special effect logic
        //                break;
        //            }
        //            // TO DO :: Add other equipment logic
        //    }
        //}

        public void OnUnEquip(EquipmentType equipmentType, StatForPlayer statData)
        {
            switch (equipmentType)
            {
                case EquipmentType.Weapon:
                    {
                        WeaponAttackTMP.text = "0.0";
                        AttackSpeedTMP.text = (Mathf.Round(statData.AttackSpeed * 10) / 10).ToString("0.0");
                        // TO DO :: Add other special effect logic
                        break;
                    }
                    // TO DO :: Add other equipment logic
            }
        }

        //public void ChangeText(StatsToDisplay stat, float newValue)
        //{
        //    switch (stat)
        //    {
        //        case StatsToDisplay.Level:
        //            {
        //                break;
        //            }
        //        case StatsToDisplay.NextExp:
        //            {
        //                NextExpTMP.text = Mathf.Round(newValue).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.CurrentExp:
        //            {
        //                CurrentExpTMP.text = Mathf.Round(newValue).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.MaxHP:
        //            {
        //                MaxHPTMP.text = Mathf.Round(newValue).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.MaxMP:
        //            {
        //                MaxMPTMP.text = Mathf.Round(newValue).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.MaxStamina:
        //            {
        //                MaxStaminaTMP.text = Mathf.Round(newValue).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.CurrentHP:
        //            {
        //                CurrentHPTMP.text = Mathf.Round(newValue).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.CurrentMP:
        //            {
        //                CurrentMPTMP.text = Mathf.Round(newValue).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.CurrentStamina:
        //            {
        //                CurrentStaminaTMP.text = Mathf.Round(newValue).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.BaseAttack:
        //            {
        //                BaseAttackTMP.text = (Mathf.Round(newValue * 10) / 10).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.WeaponAttack:
        //            {
        //                WeaponAttackTMP.text = (Mathf.Round(newValue * 10) / 10).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.BaseDefense:
        //            {
        //                BaseDefenseTMP.text = (Mathf.Round(newValue * 10) / 10).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.ArmorDefense:
        //            {
        //                ArmorDefenseTMP.text = (Mathf.Round(newValue * 10) / 10).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.AttackSpeed:
        //            {
        //                AttackSpeedTMP.text = (Mathf.Round(newValue * 10) / 10).ToString();
        //                break;
        //            }
        //        case StatsToDisplay.MovementSpeed:
        //            {
        //                MovementSpeedTMP.text = (Mathf.Round(newValue * 10) / 10).ToString();
        //                break;
        //            }
        //    }
        //}
    }
}
