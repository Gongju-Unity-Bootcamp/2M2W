using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Android;
using UnityEngine;
using TMPro;

public class AppManager : MonoBehaviour
{
    [HideInInspector] public MapSession MapSession;
    [HideInInspector] public MapRenderer MapRenderer;
    [HideInInspector] public MapInteractionController MapController;
    [HideInInspector] public MapLocationService MapLocationService;
    [HideInInspector] public MapPinLayer MapPinLayer, MapPinSubLayer, PopupPinLayer, MarkerPinLayer;
    [HideInInspector] public MapLineRenderer MapLineRenderer;

    [HideInInspector] public XROrigin XROrigin;
    [HideInInspector] public XRInteractionManager XRManager;
    [HideInInspector] public ARSession ARSession;
    [HideInInspector] public CameraService CameraService;

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
    [HideInInspector] public int index;
    [HideInInspector] public bool navMode, mute, isPinch, isOpenPopup, updateLatLon;

    [HideInInspector] public GameObject Milestone;
    [HideInInspector] public PermissionCallbacks callbacks;
    private float currentTime;
    private bool isCooldown;

    public void Init() 
    {
        GameObject BingMap = Managers.Resource.Instantiate("BingMap");
        MapSession = BingMap.GetComponent<MapSession>();
        MapRenderer = BingMap.GetComponent<MapRenderer>();
        MapController = BingMap.GetComponent<MapInteractionController>();
        MapLocationService = BingMap.GetComponent<MapLocationService>();

        GameObject subCamera = Managers.Resource.Instantiate("XROrigin");
        XROrigin = subCamera.GetComponent<XROrigin>();
        XRManager = Managers.Resource.Instantiate("XRManager").GetComponent<XRInteractionManager>();
        ARSession = subCamera.GetComponentInChildren<ARSession>();
        CameraService = subCamera.GetComponent<CameraService>();
        XROrigin.Camera.gameObject.SetActive(false);

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

        callbacks = new PermissionCallbacks();

        if (false == Permission.HasUserAuthorizedPermission(Permission.FineLocation)
        || false == Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Managers.UI.OpenPopup<PermitPopup>();
        }
        else
        {
            Managers.UI.OpenPopup<MainPopup>();
            Managers.Sound.Play(SoundID.MainBGM);
            XROrigin.Camera.gameObject.SetActive(true);
            isOpenPopup = true;
        }

        GetDeviceLocation();
        SetNavMode();
        SetMapMarker(0);
    }

    private void Update()
    {
        if (true == isOpenPopup &&
            Application.internetReachability == NetworkReachability.NotReachable)
        {
            isOpenPopup = false;
            Managers.UI.OpenPopup<InternetConnectPopup>();
        }

        if (false == updateLatLon)
        {
            return;
        }
        else
        {
            TMP_Text text = Milestone.GetComponentInChildren<TMP_Text>();
            int index = 0;

            foreach (ItineraryItem itinerary in itineraryItems)
            {
                LatLon latLon = new LatLon(itinerary.maneuverPoint.coordinates[0], itinerary.maneuverPoint.coordinates[1]);

                if (MapPin.Location.ApproximatelyEquals(latLon, 0.0004d))
                {
                    text.text = $"{index + 1}/{itineraryItems.Length}차 목적지\n방향 : {itinerary.instruction.text}\n거리 : {itinerary.travelDistance}\n예상 시간 : {itinerary.travelDuration}";
                    break;
                }

                ++index;
            }

            if (index >= itineraryItems.Length)
            {
                text.text = $"1/{itineraryItems.Length}차 목적지\n방향 : {itineraryItems[0].instruction.text}\n거리 : {itineraryItems[0].travelDistance}\n예상 시간 : {itineraryItems[0].travelDuration}";
            }
        }

        if (false == isCooldown)
        {
            TMP_Text text = Milestone.GetComponentInChildren<TMP_Text>();
            int index = 0;

            foreach (ItineraryItem itinerary in itineraryItems)
            {
                LatLon latLon = new LatLon(itinerary.maneuverPoint.coordinates[0], itinerary.maneuverPoint.coordinates[1]);

                if (MapPin.Location.ApproximatelyEquals(latLon, 0.0004d))
                {
                    text.text = $"{index + 1}/{itineraryItems.Length}차 목적지\n방향 : {itinerary.instruction.text}\n거리 : {itinerary.travelDistance}\n예상 시간 : {itinerary.travelDuration}";
                    break;
                }

                ++index;
            }

            if (index >= itineraryItems.Length)
            {
                text.text = $"1/{itineraryItems.Length}차 목적지\n방향 : {itineraryItems[0].instruction.text}\n거리 : {itineraryItems[0].travelDistance}\n예상 시간 : {itineraryItems[0].travelDuration}";
            }

            isCooldown = true;
        }
        else
        {
            currentTime += Time.deltaTime;

            if (currentTime > MapLocationService.perSeconds)
            {
                isCooldown = false;
                MapPin.Location = MapLocationService.GetLatLon();
                currentTime = 0;
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
