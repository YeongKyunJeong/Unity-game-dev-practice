using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class CustomUpdateManager : MonoBehaviour
    {
        private CustomUpdateManager instance;
        public CustomUpdateManager Instance { get => instance; private set { } }

        private List<CustomBehaviour> customBehaviours = new List<CustomBehaviour>();

        public void GenerateCustomUpdateManager()
        {
            instance = this;
            customBehaviours = new List<CustomBehaviour>();
        }

        public void AddCustomMonoBehaviour(CustomBehaviour newMember)
        {
            customBehaviours.Add(newMember);
        }

        public void RemoveCustomMonoBehaviour(CustomBehaviour newMember)
        {
            customBehaviours.Remove(newMember);
        }

        private void Awake()
        {
        }

        private void Start()
        {

        }

        private void Update()
        {

        }

        private void FixedUpdate()
        {

        }

        private void LateUpdate()
        {

        }

    }
}
