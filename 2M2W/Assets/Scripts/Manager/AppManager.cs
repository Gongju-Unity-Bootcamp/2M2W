using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [HideInInspector] public XROrigin XROrigin;
    [HideInInspector] public XRInteractionManager XRManager;

    [HideInInspector] public MapSession MapSession;
    [HideInInspector] public MapRenderer MapRenderer;
    [HideInInspector] public MapInteractionController MapController;
    [HideInInspector] public MapLocationService MapLocationService;

    [HideInInspector] public DefaultTextureTileLayer NavTile;
    [HideInInspector] public Transform cameraPivot;

    [HideInInspector] public LocationServiceStatus LocationStatus;
    [HideInInspector] public LatLon LatLon;

    [HideInInspector] public double latitude, longitude, altitude;
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

        NavTile = BingMap.GetComponent<DefaultTextureTileLayer>();
        cameraPivot = BingMap.transform.Find("CameraPivot");

        MapLocationService.GetLocation(out LocationStatus, out latitude, out longitude, out altitude);

        polatedValue = 0.025f;
        navMode = false;
        mute = false;

        Managers.UI.OpenPopup<PermitPopup>();

        LatLon originLatLon = new LatLon(36.52151612344d, 127.1728302134d);
        LatLon destinationLatLon = new LatLon(36.520442011234d, 127.17315941234d);

        StartCoroutine(originLatLon.GetRoute(destinationLatLon));
    }

    private void OnDestroy()
        => MapLocationService.StopLocationService();
}
