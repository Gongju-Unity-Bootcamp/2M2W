using Microsoft.Geospatial;
using UnityEngine;

public class MapController : MonoBehaviour
{
#if UNITY_ANDROID              
    private float initialDistance;
    private float previousDistance;
#endif
    private const float maxZoom = 18f, minZoom = 10f;

    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (mousePosition.x < 0 || mousePosition.x > screenWidth ||
            mousePosition.y < 0 || mousePosition.y > screenHeight)
        {
            return;
        }
#if UNITY_EDITOR
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            switch (scroll)
            {
                case > 0:
                    Managers.App.MapController.Zoom(Managers.App.MapRenderer.ZoomLevel + (scroll * Managers.App.polatedValue));
                    break;
                case < 0:
                    Managers.App.MapController.Zoom(-Managers.App.MapRenderer.ZoomLevel + (scroll * Managers.App.polatedValue));
                    break;
            }
        }
#elif UNITY_ANDROID
        int touch = Input.touchCount;

        if (touch == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Stationary || touch2.phase == TouchPhase.Stationary)
            {
                return;
            }

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touch1.position, touch2.position);
                previousDistance = initialDistance;
            }
            else if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                float pinchAmount = currentDistance - previousDistance;
                previousDistance = currentDistance;

                if (Mathf.Abs(pinchAmount) > 0.1f)
                {
                    switch (pinchAmount)
                    {
                        case > 0:
                            Managers.App.MapController.Zoom(pinchAmount * Managers.App.polatedValue);
                            break;
                        case < 0:
                            Managers.App.MapController.Zoom(pinchAmount * Managers.App.polatedValue);
                            break;
                    }

                    Managers.App.isPinch = true;
                }
                else
                {
                    Managers.App.isPinch = false;
                }
            }
        }
#endif
        switch (Managers.App.MapRenderer.ZoomLevel)
        {
            case >= maxZoom:
                Managers.App.MapRenderer.ZoomLevel = maxZoom;
                break;
            case <= minZoom:
                Managers.App.MapRenderer.ZoomLevel = minZoom;
                break;
        }
    }
}
