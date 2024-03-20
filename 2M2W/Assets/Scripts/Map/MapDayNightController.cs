using System;
using UnityEngine;

public class MapDayNightController : MonoBehaviour
{
    private Light directionalLight;
    private const float dayIntensity = 1.0f, nightIntensity = 0.2f;

    private void Start()
        => directionalLight = GetComponent<Light>();

    private void Update()
    {
        DateTime currentTime = DateTime.Now;
        float normalizedHour = (float)currentTime.Hour + ((float)currentTime.Minute / 60f);

        directionalLight.intensity = CalculateLightIntensity(normalizedHour);
    }

    private float CalculateLightIntensity(float hour)
    {
        float lerpFactor = Mathf.Clamp01((hour - 6f) / 12f);

        return Mathf.Lerp(nightIntensity, dayIntensity, lerpFactor);
    }
}