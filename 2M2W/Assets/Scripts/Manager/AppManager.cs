using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.Android;

public class AppManager : MonoBehaviour
{
    [HideInInspector] public XROrigin XROrigin;
    [HideInInspector] public XRInteractionManager XRManager;

    [HideInInspector] public MapSession MapSession;
    [HideInInspector] public MapRenderer MapRenderer;
    [HideInInspector] public MapInteractionController MapController;
    [HideInInspector] public MapLocationService MapLocationService;
    [HideInInspector] public MapPinLayer MapPinLayer;

    [HideInInspector] public Camera MapCamera;

    [HideInInspector] public DefaultTextureTileLayer NavTile;

    [HideInInspector] public LocationServiceStatus LocationStatus;
    [HideInInspector] public LatLonAlt latLonAlt;
    [HideInInspector] public LatLon startLatLon, endLatLon;
    [HideInInspector] public ItineraryItem[] itineraryItems;

    [HideInInspector] public float polatedValue;
    [HideInInspector] public bool navMode, mute, isPinch;

    public void Init() 
    {
        XROrigin = Managers.Resource.Instantiate("XROrigin").GetComponent<XROrigin>();
        XRManager = Managers.Resource.Instantiate("XRManager").GetComponent<XRInteractionManager>();

        GameObject BingMap = Managers.Resource.Instantiate("BingMap");
        MapSession = BingMap.GetComponent<MapSession>();
        MapRenderer = BingMap.GetComponent<MapRenderer>();
        MapController = BingMap.GetComponent<MapInteractionController>();
        MapLocationService = BingMap.GetComponent<MapLocationService>();
        MapPinLayer = BingMap.GetComponent<MapPinLayer>();

        MapCamera = BingMap.transform.Find("MapCamera").GetComponent<Camera>();

        NavTile = BingMap.GetComponent<DefaultTextureTileLayer>();

        if (MapLocationService.GetLocation(out LocationServiceStatus status, out double latitude, out double longitude, out double altitude))
        {
            LocationStatus = status;
            latLonAlt = new LatLonAlt(latitude, longitude, altitude);
        }
        else
        {
            latLonAlt = default;
        }

        polatedValue = 0.025f;
        navMode = false;
        mute = false;

        if (false == Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            || false == Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Managers.UI.OpenPopup<PermitPopup>();
        }
        else
        {
            Managers.UI.OpenPopup<MainPopup>();
            Managers.Sound.Play(SoundID.MainBGM);
        }
    }

    public void GetNavMode()
    {
        if (false == navMode)
        {
            navMode = true;
            NavTile.ImageryType = MapImageryType.Symbolic;
            NavTile.ImageryStyle = MapImageryStyle.Vibrant;
        }
        else
        {
            navMode = false;
            NavTile.ImageryType = MapImageryType.Aerial;
        }
    }

    private void OnDestroy()
        => MapLocationService.StopLocationService();
}
