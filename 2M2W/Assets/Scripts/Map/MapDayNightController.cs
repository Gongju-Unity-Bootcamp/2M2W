using System;
using UnityEngine;

public class MapDayNightController : MonoBehaviour
{
    private Light directionalLight;
    public Vector3 dayRotation = new Vector3(0f, 0f, 0f);
    public Vector3 nightRotation = new Vector3(180f, 0f, 0f);

    private const float dayIntensity = 1.0f, nightIntensity = 0.2f;

    private void Start()
        => directionalLight = GetComponent<Light>();

    private void Update()
    {
        DateTime currentTime = DateTime.Now;

        float normalizedHour = (float)currentTime.Hour + ((float)currentTime.Minute / 60f);

        Vector3 targetRotation = CalculateRotation(normalizedHour);
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);

        directionalLight.transform.rotation = Quaternion.Lerp(directionalLight.transform.rotation, targetQuaternion, Time.deltaTime);
        directionalLight.intensity = CalculateLightIntensity(normalizedHour);
    }

    private Vector3 CalculateRotation(float hour)
    {
        float lerpFactor = Mathf.Clamp01((hour - 6f) / 12f);

        return Vector3.Lerp(nightRotation, dayRotation, lerpFactor);
    }

    private float CalculateLightIntensity(float hour)
    {
        float lerpFactor = Mathf.Clamp01((hour - 6f) / 12f);

        return Mathf.Lerp(nightIntensity, dayIntensity, lerpFactor);
    }
}