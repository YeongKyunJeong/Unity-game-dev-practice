using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDP
{

    public class Node : MonoBehaviour
    {
        private BuildManager buildManager;

        public Color hoverColor;
        public Color initColor;
        public Color notEnoughMoneyColor;
        private static Vector3 positionOffset;


        private GameObject turretGOOnMe;
        private TurretBluePrint turretOnMeBP;
        public TurretBluePrint GetTurretBP { get => turretOnMeBP; }
        public bool isUpgraded = false;

        private Renderer rend;

        public void Initialize()
        {
            if (buildManager == null)
                buildManager = BuildManager.Instance;

            if (rend == null)
            {
                rend = GetComponent<Renderer>();
            }
            rend.material.color = initColor;

            isUpgraded = false;
            positionOffset = 0.5f * Vector3.up;
        }

        public Vector3 GetBuildPosition()
        {
            return transform.position + positionOffset;
        }

        private void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!buildManager.CanBuild)
            {
                return;
            }

            if (buildManager.GetTurretToBuild != null)
            {
                if (buildManager.HasEnoughMoney)
                {
                    rend.material.color = hoverColor;
                }
                else
                {
                    rend.material.color = notEnoughMoneyColor;
                }
            }

        }

        public void BuildTurretOnMe(TurretBluePrint turretBP)
        {
            if (PlayerStats.Money < turretBP.cost)
            {
                Debug.Log("Not Enough Money");
                return;
            }

            //playerStat.Money = 
            PlayerStats.Money -= turretBP.cost;
            Debug.Log("Turret Built!");

            turretGOOnMe = Instantiate(turretBP.prefab, GetBuildPosition(), Quaternion.identity);
            turretOnMeBP = turretBP;
            Destroy(Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity), 2f);
        }

        public void UpgradeTurret()
        {
            if (PlayerStats.Money < turretOnMeBP.upgradeCost)
            {
                Debug.Log("Not Enough Money to Upgrade");
                return;
            }

            //playerStat.Money = 
            PlayerStats.Money -= turretOnMeBP.upgradeCost;

            Destroy(turretGOOnMe);  // Get rid of old turret

            Debug.Log("Turret Upgraded!");
            isUpgraded = true;
            // Build new turret
            turretGOOnMe = Instantiate(turretOnMeBP.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
            Destroy(Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity), 2f);
        }

        public void SellTurret()
        {
            PlayerStats.Money += turretOnMeBP.GetSellAmount();
            Destroy(turretGOOnMe);
            Destroy(Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity), 2f);
            turretOnMeBP = null;
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (turretGOOnMe != null)
            {
                buildManager.SelectNode(this);
                return;
            }

            if (!buildManager.CanBuild)
            {
                return;
            }

            BuildTurretOnMe(buildManager.GetTurretToBuild);
        }

        private void OnMouseExit()
        {
            rend.material.color = initColor;
        }
    }
}
