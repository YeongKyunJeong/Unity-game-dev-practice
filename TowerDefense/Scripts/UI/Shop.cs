using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TDP
{

    public class Shop : MonoBehaviour
    {
        [SerializeField] private TurretBluePrint standardTurretBP;
        [SerializeField] private TurretBluePrint missileTurretBP;
        [SerializeField] private TurretBluePrint laserTurretBP;

        private GameManager gameManager;
        private BuildManager buildManager;

        public void Initialize()
        {
            gameManager = GameManager.Instance;
            buildManager = BuildManager.Instance;
        }

        public void SelectStandardTurret()
        {
            buildManager.SelectTurretToBuild(standardTurretBP);
        }
        public void SelectMissileTurret()
        {
            buildManager.SelectTurretToBuild(missileTurretBP);
        }
        public void SelectLaserTurret()
        {
            buildManager.SelectTurretToBuild(laserTurretBP);
        }
    }

}