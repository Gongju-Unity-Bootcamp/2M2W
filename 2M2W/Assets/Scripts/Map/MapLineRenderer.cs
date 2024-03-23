using Microsoft.Maps.Unity;
using UnityEngine;

public class MapLineRenderer : MonoBehaviour
{
    private ObservableList<MapPin> mapPins;
    private LineRenderer lineRenderer;
    private float thickness = 0.3f;

    private void Awake()
    {
        mapPins = Managers.App.MapPinSubLayer.MapPins;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, thickness);
        curve.AddKey(1f, thickness);

        lineRenderer.widthCurve = curve;
    }

    private void OnEnable()
        => Managers.App.MapRenderer.ZoomLevel = 1f;

    private void Update()
    {
        if (mapPins.Count > 0)
        {
            int activeMapPinCount = 0;

            for (int index = 0; index < mapPins.Count; ++index)
            {
                if (mapPins[index].enabled)
                {
                    if (activeMapPinCount >= lineRenderer.positionCount)
                    {
                        ++lineRenderer.positionCount;
                    }

                    lineRenderer.SetPosition(activeMapPinCount, new Vector3(mapPins[index].transform.position.x, 20f, mapPins[index].transform.position.z));
                    ++activeMapPinCount;
                }
            }
        }
    }

    private void OnDisable()
        => lineRenderer.positionCount = 0;
}
