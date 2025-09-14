using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDP
{
    public class NodeUI : MonoBehaviour
    {
        private BuildManager buildManager;
        private Node target;
        [SerializeField] private GameObject uIGO;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Text upgradeCostText;
        [SerializeField] private Text sellAmountText;

        public void Initialize()
        {
            if (buildManager == null)
            {
                buildManager = BuildManager.Instance;
            }
            upgradeCostText.text = string.Empty;
            sellAmountText.text = string.Empty;
            HideUI();
        }

        public void SetTarget(Node _target)
        {
            target = _target;
            transform.position = target.GetBuildPosition();
            if (target.isUpgraded)
            {
                upgradeCostText.text = $"Done";
                upgradeButton.interactable = false;
            }
            else
            {
                upgradeCostText.text = $"$ {target.GetTurretBP.upgradeCost}";
                upgradeButton.interactable = true;
            }
            sellAmountText.text = $"$ {target.GetTurretBP.GetSellAmount()}";
            uIGO.SetActive(true);
        }

        public void HideUI()
        {
            uIGO.SetActive(false);
        }

        public void UpgradeCall()
        {
            target.UpgradeTurret();
            buildManager.DeselectNode();
        }

        public void SellCall()
        {
            target.SellTurret();
            buildManager.DeselectNode();
        }
    }
}
