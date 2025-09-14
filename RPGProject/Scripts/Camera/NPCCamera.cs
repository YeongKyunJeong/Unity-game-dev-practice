using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class NPCCamera : MonoBehaviour
    {
        [field: SerializeField] private CinemachineVirtualCamera virtualCamera { get; set; }
        public CinemachineVirtualCamera VirtualCamera 
        {
            get 
            {
                if (virtualCamera == null)
                {
                    virtualCamera = GetComponent<CinemachineVirtualCamera>();
                }

                return virtualCamera;
            }
        }


    }
}
