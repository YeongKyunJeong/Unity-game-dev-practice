using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace RSP2
{
    public class CameraZoomer : MonoBehaviour
    {
        [SerializeField] [Range(0, 10f)] private float defualtDistance = 6f;
        [SerializeField] [Range(0, 10f)] private float minimumDistance = 1f;
        [SerializeField] [Range(0, 10f)] private float maximumDistance = 6f;

        [SerializeField] [Range(0f, 10f)] private float smoothing = 4f;
        [SerializeField] [Range(0f, 10f)] private float zoomSensitivity = 6f;

        private CinemachineFramingTransposer framingTransposer;
        private CinemachineInputProvider inputProvider;

        private float targetCameraDistance;

        private float zoomValue;
        private float currentCameraDistance;
        private float lerpedZoomValue;

        private void Awake()
        {
            framingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
            inputProvider = GetComponent<CinemachineInputProvider>();

            targetCameraDistance = defualtDistance;
            currentCameraDistance = defualtDistance;
        }

        public void Update()
        {
            Zoom();
        }

        private void ZoomIstantly(float targetValue)
        {
            framingTransposer.m_CameraDistance = targetValue;
        }

        private void Zoom()
        {
            zoomValue = inputProvider.GetAxisValue(2) ;

            zoomValue *= -zoomSensitivity;

            targetCameraDistance = Mathf.Clamp(targetCameraDistance + zoomValue, minimumDistance, maximumDistance);

            currentCameraDistance = framingTransposer.m_CameraDistance;
            if (currentCameraDistance == targetCameraDistance)
            {
                return;
            }

            lerpedZoomValue = Mathf.Lerp(currentCameraDistance, targetCameraDistance, smoothing * Time.deltaTime);

            framingTransposer.m_CameraDistance = lerpedZoomValue;
        }
    }
}
