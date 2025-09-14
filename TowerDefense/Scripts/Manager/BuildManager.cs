using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{

    public class BuildManager : MonoBehaviour
    {
        private static BuildManager instance;
        public static BuildManager Instance { get { return instance; } private set { instance = value; } }

        private GameManager gameManager;
        //private PlayerStats playerStats;

        public GameObject buildEffect;
        public GameObject sellEffect;

        [SerializeField] private TurretBluePrint turretToBuild;
        public TurretBluePrint GetTurretToBuild { get => turretToBuild; }
        private Node selectedNode;

        [SerializeField] private NodeUI nodeUI;

        public bool CanBuild { get { return turretToBuild != null; } }
        public bool HasEnoughMoney { get { return PlayerStats.Money >= turretToBuild.cost; } }

        public void Initialize()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }
            //playerStats = gameManager.GetPlayerStats;

            if (instance == null)
            {
                Instance = this;
            }

            if(nodeUI == null)
            {
                Debug.Log("Node UI not assigned");
            }

            nodeUI.Initialize();
            turretToBuild = null;
        }


        public void SelectNode(Node node)
        {
            if(selectedNode == node)
            {
                DeselectNode();
                return;
            }

            selectedNode = node;
            turretToBuild = null;

            nodeUI.SetTarget(node);
        }

        public void DeselectNode()
        {
            selectedNode = null;
            nodeUI.HideUI();
        }

        public void SelectTurretToBuild(TurretBluePrint turretBP)
        {
            turretToBuild = turretBP;
            DeselectNode();
        }

        //public TurretBluePrint GetTurretToBuild()
        //{
        //    return turretToBuild;
        //}
    }
}
