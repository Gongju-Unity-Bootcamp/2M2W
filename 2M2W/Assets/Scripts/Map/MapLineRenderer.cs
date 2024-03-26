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
        => lineRenderer.positionCount = mapPins.Count;

    private void Update()
    {
        lineRenderer.positionCount = 0;

        if (mapPins.Count > 0)
        {
            for (int index = 0; index < mapPins.Count; ++index)
            {
                if (true == mapPins[index].enabled)
                {
                    lineRenderer.SetPosition(lineRenderer.positionCount++, new Vector3(mapPins[index].transform.position.x, mapPins[index].transform.position.y, mapPins[index].transform.position.z));
                }
            }
        }
    }

    private void OnDisable()
    {
        mapPins.Clear();
        lineRenderer.positionCount = 0;
    }
}
