using Microsoft.Geospatial;
using UniRx;
using System.Collections;
using System;
using Object = UnityEngine.Object;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    public static T FindChild<T>(GameObject gameObject, string name = null, bool recursive = false) where T : Object
        => Utilities.FindChild<T>(gameObject, name, recursive);

    public static GameObject FindChild(GameObject gameObject, string name = null, bool recursive = false)
        => Utilities.FindChild<Transform>(gameObject, name, recursive).gameObject;

    public static void BindViewEvent(this UIBehaviour view, Action<PointerEventData> action, ViewEvent type, Component component)
        => Utilities.BindViewEvent(view, action, type, component);

    public static void BindModelEvent<T>(this ReactiveProperty<T> model, Action<T> action, Component component)
        => Utilities.BindModelEvent(model, action, component);

    public static SoundType ConvertToSoundType(this SoundID id)
    {
        if (id > SoundID.VFX)
        {
            return SoundType.VFX;
        }
        else
        {
            return SoundType.BGM;
        }
    }

    public static Ray GetRay(this PointerEventData eventData)
    {
        RawImage rawImage = eventData.pointerEnter.GetComponent<RawImage>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform, eventData.position, null, out Vector2 localPoint);

        Vector3 viewportPoint = rawImage.rectTransform.TransformPoint(localPoint) / Screen.height;
        Vector2 textureCoord = new Vector2(viewportPoint.x * Managers.App.MapCamera.targetTexture.width, viewportPoint.y * Managers.App.MapCamera.targetTexture.height);

        return Managers.App.MapCamera.ScreenPointToRay(textureCoord);
    }

    public static IEnumerator GetRoute(this LatLon originLanLot, LatLon destinationLanLot, BingRouteMode routeMode)
    {
        UnityWebRequest www = default;

        switch (routeMode)
        {
            case BingRouteMode.Walking:
                www = UnityWebRequest.Get(BingMap.GetWalkingUrl(originLanLot, destinationLanLot));
                break;
            case BingRouteMode.Driving:
                www = UnityWebRequest.Get(BingMap.GetDrivingUrl(originLanLot, destinationLanLot));
                break;
        }

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            string response = www.downloadHandler.text;
            Debug.Log("Response: " + response);
        }
    }
}
