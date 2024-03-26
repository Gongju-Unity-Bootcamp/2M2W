using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Android;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [HideInInspector] public XROrigin XROrigin;
    [HideInInspector] public XRInteractionManager XRManager;
    [HideInInspector] public CameraService CameraService;

    [HideInInspector] public MapSession MapSession;
    [HideInInspector] public MapRenderer MapRenderer;
    [HideInInspector] public MapInteractionController MapController;
    [HideInInspector] public MapLocationService MapLocationService;
    [HideInInspector] public MapPinLayer MapPinLayer, MapPinSubLayer, PopupPinLayer, MarkerPinLayer;
    [HideInInspector] public MapLineRenderer MapLineRenderer;

    [HideInInspector] public Camera MapCamera;

    [HideInInspector] public DefaultTextureTileLayer NavTile;

    [HideInInspector] public LocationServiceStatus LocationStatus;
    [HideInInspector] public BingRouteMode BingRouteMode;
    [HideInInspector] public LatLonAlt latLonAlt;
    [HideInInspector] public LatLon startLatLon, endLatLon;
    [HideInInspector] public ItineraryItem[] itineraryItems;
    [HideInInspector] public MarkerData MarkerData;
    [HideInInspector] public MapPin MapPin;

    [HideInInspector] public float polatedValue;
    [HideInInspector] public bool navMode, mute, isPinch, updateLatLon;

    private float currentTime;
    private bool isCooldown;

    public void Init() 
    {
        GameObject subCamera = Managers.Resource.Instantiate("XROrigin");
        XROrigin = subCamera.GetComponent<XROrigin>();
        XRManager = Managers.Resource.Instantiate("XRManager").GetComponent<XRInteractionManager>();
        CameraService = subCamera.GetComponent<CameraService>();

        GameObject BingMap = Managers.Resource.Instantiate("BingMap");
        MapSession = BingMap.GetComponent<MapSession>();
        MapRenderer = BingMap.GetComponent<MapRenderer>();
        MapController = BingMap.GetComponent<MapInteractionController>();
        MapLocationService = BingMap.GetComponent<MapLocationService>();
        MapPinLayer = BingMap.GetComponent<MapPinLayer>();
        MapPinSubLayer = BingMap.AddComponent<MapPinLayer>();
        PopupPinLayer = BingMap.AddComponent<MapPinLayer>();
        MarkerPinLayer = BingMap.AddComponent<MapPinLayer>();
        MapLineRenderer = Managers.Resource.Instantiate("MapLineRenderer").GetComponent<MapLineRenderer>();

        MapPinLayer.LayerName = nameof(MapPinLayer);
        MapPinSubLayer.LayerName = nameof(MapPinSubLayer);
        PopupPinLayer.LayerName = nameof(PopupPinLayer);
        MarkerPinLayer.LayerName = nameof(MarkerPinLayer);

        MapLineRenderer.gameObject.SetActive(false);
        MapCamera = BingMap.transform.Find("MapCamera").GetComponent<Camera>();

        NavTile = BingMap.GetComponent<DefaultTextureTileLayer>();

        polatedValue = 0.025f;
        navMode = false;
        mute = false;

        if (false == Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            || false == Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            MapLocationService.StartLocationService();
            CameraService.StartCameraService();
            Managers.UI.OpenPopup<PermitPopup>();
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Managers.UI.OpenPopup<InternetConnectPopup>();
        }

        Managers.UI.OpenPopup<MainPopup>();
        Managers.Sound.Play(SoundID.MainBGM);

        GetDeviceLocation();
        SetNavMode();
        SetMapMarker(0);
    }

    private void Update()
    {
        if (true == updateLatLon)
        {
            if (false == isCooldown)
            {
                isCooldown = true;
                currentTime += Time.deltaTime;

                if (currentTime > MapLocationService.perSeconds)
                {
                    isCooldown = false;
                    MapPin.Location = MapLocationService.GetLatLon();
                    currentTime = 0;
                }
            }
        }
    }

    public void GetDeviceLocation()
    {
        if (MapLocationService.GetLocation(out LocationServiceStatus status, out double latitude, out double longitude, out double altitude))
        {
            LocationStatus = status;
            latLonAlt = new LatLonAlt(latitude, longitude, altitude);
        }
        else
        {
            latLonAlt = default;
        }
    }

    public void SetNavMode()
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

    public void SetMapMarker(int index)
    {
        foreach (MarkerData marker in Managers.Data.Marker.Values)
        {
            if (index == 0)
            {
                MapPin mapPin = Managers.Resource.Instantiate("MarkerPin").GetComponent<MapPin>();
                mapPin.gameObject.GetComponent<MapMarkerPinController>().SetMarkerPin(marker);
                mapPin.Location = new LatLon(marker.Latitude, marker.Longitude);
                mapPin.IsLayerSynchronized = false;
                MarkerPinLayer.MapPins.Add(mapPin);
            }
            else
            {
                if (index == marker.Group)
                {
                    MapPin mapPin = Managers.Resource.Instantiate("MarkerPin").GetComponent<MapPin>();
                    mapPin.gameObject.GetComponent<MapMarkerPinController>().SetMarkerPin(marker);
                    mapPin.Location = new LatLon(marker.Latitude, marker.Longitude);
                    mapPin.IsLayerSynchronized = false;
                    MarkerPinLayer.MapPins.Add(mapPin);
                }
            }
        }
    }

    public void EnableMarkerPin(int index)
    {
        MarkerPinLayer.MapPins.Clear();
        SetMapMarker(index);
    }
}
