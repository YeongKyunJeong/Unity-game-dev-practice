using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RSP2
{
    public class DayNightManager : MonoBehaviour
    {
        //[field: SerializeField] private string sunMoonParentName { get; set; } 

        [Range(0f, 1f)] public float TimeCycle;
        [field: SerializeField] private bool isDayTime { get; set; }
        private bool isDayNightChanged;
        [field: SerializeField] public float StartTime { get; private set; }
        [field: SerializeField] public bool isSunOnly { get; private set; }

        [Header("Day")]
        [field: SerializeField] private float dayLength;
        public float DayLength => dayLength;
        private float dayTimeRate;
        private Light sun;
        [field: SerializeField] private Gradient sunGradient;
        [field: SerializeField] private AnimationCurve sunIntensityCurve;

        [Header("Night")]
        [field: SerializeField] private float nightLength;
        public float NightLength => nightLength;
        private float nightTimeRate;
        private Light moon;
        [field: SerializeField] private Gradient moonGradient;
        [field: SerializeField] private AnimationCurve moonIntensityCurve;

        [Header("Others")]
        public Vector3 PolarisDirection;
        public AnimationCurve AmbientIntensityCurve;
        public AnimationCurve ReflectionIntensityCurve;

        private Material skyBoxMaterial;
        private bool isInitialized;

        public void Initialize()
        {
            SunAndMoon sunAndMoon = FindObjectOfType<SunAndMoon>();
            if (sunAndMoon == null)
            {
                Debug.LogError("Sun and Moon not found");
                return;
            }

            skyBoxMaterial = new Material(RenderSettings.skybox);
            RenderSettings.skybox = skyBoxMaterial;

            sun = sunAndMoon.Sun;
            if (dayLength < 1)
            {
                Debug.LogError("Day length is 0");
                return;
            }
            dayTimeRate = 1f / dayLength;

            if (isSunOnly)
            {
                RenderSettings.sun = sun;

                if (RenderSettings.skybox.HasProperty("_Exposure"))
                {
                    RenderSettings.skybox.SetFloat("_Exposure", 1.3f);
                }

                if (RenderSettings.skybox.HasProperty("_AtmosphereThickness"))
                {
                    RenderSettings.skybox.SetFloat("_AtmosphereThickness", 1f);
                }

            }
            else
            {
                moon = sunAndMoon.Moon;
                if (nightLength < 1)
                {
                    Debug.LogError("Night length is 0");
                    return;
                }
                //UpdateEnvironmentLighting(true);

            }

            nightTimeRate = 1f / nightLength;
            TimeCycle = isDayTime ? StartTime / dayLength : StartTime / nightLength;


            if (TimeCycle != 0) isDayNightChanged = false;
            else isDayNightChanged = true;

            UpdateTime();
            UpdateSkyLightReference(true);
            UpdateSkyboxExposure(true);
            UpdateEnvironmentLighting(true);
        }

        public void CallPhysicsUpdate()
        {
            //if (!isInitialized) return;

            UpdateTime();
            UpdateSkyLightReference();

            if (isSunOnly)
            {
                UpdateLighting(sun, sunGradient, sunIntensityCurve);
            }
            else
            {
                if (isDayTime)
                {
                    UpdateLighting(sun, sunGradient, sunIntensityCurve);
                }
                else
                {
                    UpdateLighting(moon, moonGradient, moonIntensityCurve);
                }
            }

            UpdateSkyboxExposure();
            UpdateEnvironmentLighting();
        }

        private void UpdateTime()
        {
            if (isDayNightChanged) isDayNightChanged = false;

            if (isDayTime)
            {
                TimeCycle = (TimeCycle + dayTimeRate * Time.fixedDeltaTime);
            }
            else
            {
                TimeCycle = (TimeCycle + nightTimeRate * Time.fixedDeltaTime);
            }

            if (TimeCycle >= 1)
            {
                TimeCycle -= 1;
                isDayTime = !isDayTime;
                isDayNightChanged = true;
            }
        }

        private void UpdateLighting(Light lightSource, Gradient colorGradient, AnimationCurve intensityCurve)
        {
            float intensity = 0;

            if (isSunOnly && !isDayTime)
            {
                lightSource.transform.eulerAngles = (TimeCycle + 1) * 2f * PolarisDirection;
                lightSource.color = colorGradient.Evaluate(0);
                intensity = intensityCurve.Evaluate(0);
            }

            else
            {
                lightSource.transform.eulerAngles = TimeCycle * 2f * PolarisDirection;
                lightSource.color = colorGradient.Evaluate(TimeCycle);
                intensity = intensityCurve.Evaluate(TimeCycle);
            }

            lightSource.intensity = intensity;
        }


        private void UpdateSkyLightReference(bool isInitializing = false)
        {
            if (isSunOnly) return;

            if (!isDayNightChanged && !isInitializing) return;


            if (isDayTime)
            {
                RenderSettings.sun = sun;
                sun.gameObject.SetActive(true);
                //moon.gameObject.SetActive(false);
            }
            else
            {
                RenderSettings.sun = moon;
                //moon.gameObject.SetActive(false);
                sun.gameObject.SetActive(false);
            }
        }

        private void UpdateSkyboxExposure(bool isInitializing = false)
        {
            if (isSunOnly) return;

            if (RenderSettings.skybox == null) return;

            if (!isDayNightChanged && !isInitializing) return;

            if (RenderSettings.skybox.HasProperty("_Exposure"))
            {
                float targetExposure = isDayTime ? 1.3f : 0.3f;
                RenderSettings.skybox.SetFloat("_Exposure", targetExposure);
            }

            if (RenderSettings.skybox.HasProperty("_AtmosphereThickness"))
            {
                float targetThickness = isDayTime ? 1f : 0.1f;
                RenderSettings.skybox.SetFloat("_AtmosphereThickness", targetThickness);
            }
        }

        private void UpdateEnvironmentLighting(bool isInitializing = false)
        {
            if (isDayTime)
            {
                RenderSettings.ambientIntensity = AmbientIntensityCurve.Evaluate(TimeCycle);
                RenderSettings.reflectionIntensity = ReflectionIntensityCurve.Evaluate(TimeCycle);

                return;
            }

            if (isDayNightChanged || isInitializing)
            {
                RenderSettings.ambientIntensity = AmbientIntensityCurve.Evaluate(0);
                RenderSettings.reflectionIntensity = ReflectionIntensityCurve.Evaluate(0);
            }
        }
    }
}
