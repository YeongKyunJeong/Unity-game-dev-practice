using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        private InGameManager gameManager;
        private Camera mainCamera;
        private CinemachineVirtualCamera playerCamera;
        private CinemachineBrain cinemachineBrain;
        private CinemachineBasicMultiChannelPerlin playerNoise;

        private HashSet<CinemachineVirtualCamera> virtualCameras;

        private Coroutine cameraShakeCoroutine;

        [field: SerializeField] private float playerCameraPriority { get; set; }
        [field: SerializeField] private Vector2Int nPCCameraPriority { get; set; }

        [field: SerializeField] private CinemachineVirtualCamera currentCamera { get; set; }
        [field: SerializeField] private Vector2 maxShakingValue { get; set; }
        [field: SerializeField] private float maxShakingTime { get; set; }

        public void Initialize(InGameManager _gameManager)
        {
            gameManager = _gameManager;

            cinemachineBrain = Camera.main.transform.GetComponent<CinemachineBrain>();
            virtualCameras = new HashSet<CinemachineVirtualCamera>();
            if (SearchPlayerCamera())
            {
                currentCamera = playerCamera;
            }

        }

        public void AddCamera(CinemachineVirtualCamera newCamera, bool nowChange = false, bool immediatelyChange = false)
        {
            if (!virtualCameras.Contains(newCamera))
            {
                virtualCameras.Add(newCamera);
                if (nowChange)
                {
                    SetBlendTime(immediatelyChange);
                    newCamera.Priority = nPCCameraPriority.y;
                }
                else newCamera.Priority = nPCCameraPriority.x;

            }
        }

        private void SetBlendTime(bool immediatelyChange)
        {
            if (immediatelyChange)
            {
                cinemachineBrain.m_DefaultBlend.m_Time = 0;
            }
            else
            {
                cinemachineBrain.m_DefaultBlend.m_Time = 1;
            }
        }

        public void RemoveCamera(CinemachineVirtualCamera targetCamera, bool immediatelyChange = false)
        {
            if (currentCamera == targetCamera)
            {
                SetBlendTime(immediatelyChange);
                ResetToPlayerCamera();
            }

            if (virtualCameras.Contains(targetCamera))
            {
                virtualCameras.Remove(targetCamera);
            }
        }

        public void CallCameraSwitching(CinemachineVirtualCamera targetCamera, bool immediatelyChange = false)
        {
            SetBlendTime(immediatelyChange);

            if (targetCamera == null)
            {

                ResetToPlayerCamera();
                return;
            }

            if (!virtualCameras.Contains(targetCamera))
            {
                virtualCameras.Add(targetCamera);
            }

            SwitchToNewCamera(targetCamera);
        }

        private bool SearchPlayerCamera()
        {
            mainCamera = Camera.main;
            playerCamera = FindObjectOfType<CameraZoomer>().transform.GetComponent<CinemachineVirtualCamera>();

            if (playerCamera == null) return false;

            playerNoise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            playerNoise.enabled = true;
            playerNoise.m_AmplitudeGain = 0;
            playerNoise.m_FrequencyGain = 0;
            return true;

        }

        private void SwitchToNewCamera(CinemachineVirtualCamera newCamera)
        {
            if (currentCamera != playerCamera)
            {
                currentCamera.Priority = nPCCameraPriority.x;
            }

            newCamera.Priority = nPCCameraPriority.y;
            currentCamera = newCamera;
        }

        private void ResetToPlayerCamera()
        {
            if (currentCamera == playerCamera) return;

            currentCamera.Priority = nPCCameraPriority.x;

            currentCamera = playerCamera;
        }

        public void CallCameraShakeByHit(float intensity)
        {
            if (cameraShakeCoroutine != null)
            {
                StopCoroutine(cameraShakeCoroutine);
            }

            cameraShakeCoroutine = StartCoroutine(ShakeCameraByHit(intensity));
        }

        private IEnumerator ShakeCameraByHit(float intensity)
        {
            playerNoise.m_AmplitudeGain = intensity * maxShakingValue.x;
            playerNoise.m_FrequencyGain = maxShakingValue.y;
            yield return new WaitForSeconds(intensity * maxShakingTime);

            playerNoise.m_AmplitudeGain = 0;
            playerNoise.m_FrequencyGain = 0;

            yield return null;
        }

    }
}
