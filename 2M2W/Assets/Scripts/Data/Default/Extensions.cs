using Microsoft.Geospatial;
using UniRx;
using System.Collections;
using System;
using Object = UnityEngine.Object;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

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
        localPoint.x = (rawImage.rectTransform.rect.xMin * -1) - (localPoint.x * -1);
        localPoint.y = (rawImage.rectTransform.rect.yMin * -1) - (localPoint.y * -1);
        Vector2 viewportPoint = new Vector2(localPoint.x / rawImage.rectTransform.rect.size.x, localPoint.y / rawImage.rectTransform.rect.size.y);

        return Managers.App.MapCamera.ViewportPointToRay(new Vector3(viewportPoint.x, viewportPoint.y, 0));
    }

    public static IEnumerator GetRoute(this LatLon originLatLon, LatLon destinationLatLon, Action<string> callback, BingRouteMode routeMode = BingRouteMode.Walking)
    {
        UnityWebRequest www = default;

        switch (routeMode)
        {
            case BingRouteMode.Walking:
                www = UnityWebRequest.Get(BingMap.GetWalkingUrl(originLatLon, destinationLatLon));
                break;
            case BingRouteMode.Driving:
                www = UnityWebRequest.Get(BingMap.GetDrivingUrl(originLatLon, destinationLatLon));
                break;
            case BingRouteMode.Transit:
                www = UnityWebRequest.Get(BingMap.GetTransitUrl(originLatLon, destinationLatLon));
                break;
            case BingRouteMode.Bicycling:
                www = UnityWebRequest.Get(BingMap.GetBicyclingUrl(originLatLon, destinationLatLon));
                break;
        }

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            callback(www.error);
        }
        else
        {
            callback(www.downloadHandler.text);
        }
    }

    public static IEnumerator GetLatLon(this string addressName, Action<string> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(BingMap.GetLatLonUrl(addressName));

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            callback(www.error);
        }
        else
        {
            callback(www.downloadHandler.text);
        }
    }

    public static IEnumerator GetAddress(this LatLon originLatLon, Action<string> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(BingMap.GetAddressUrl(originLatLon));

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            callback(www.error);
        }
        else
        {
            callback(www.downloadHandler.text);
        }
    }

    public static IEnumerator GetSearchAddress(this string addressName, Action<string> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get(BingMap.GetSearchAddressUrl(addressName));

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            callback(www.error);
        }
        else
        {
            callback(www.downloadHandler.text);
        }
    }
}
