using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [HideInInspector] public XROrigin XROrigin;
    [HideInInspector] public XRInteractionManager XRManager;

    [HideInInspector] public MapRenderer MapRenderer;
    [HideInInspector] public MapInteractionController MapController;
    [HideInInspector] public MapLocationService MapLocationService;
    [HideInInspector] public DefaultTextureTileLayer NavTile;

    [HideInInspector] public Transform cameraPivot;

    [HideInInspector] public LocationServiceStatus LocationStatus;
    [HideInInspector] public LatLon LatLon;

    [HideInInspector] public double latitude, longitude, altitude;
    [HideInInspector] public float polatedValue;
    [HideInInspector] public bool navMode;
    [HideInInspector] public bool mute;

    public void Init() 
    {
        XROrigin = Managers.Resource.Instantiate("XROrigin").GetComponent<XROrigin>();
        XRManager = Managers.Resource.Instantiate("XRManager").GetComponent<XRInteractionManager>();

        GameObject BingMap = Managers.Resource.Instantiate("BingMap");
        MapRenderer = BingMap.GetComponent<MapRenderer>();
        MapController = BingMap.GetComponent<MapInteractionController>();
        NavTile = BingMap.GetComponent<DefaultTextureTileLayer>();

        cameraPivot = BingMap.transform.Find("CameraPivot");

        MapLocationService = Managers.Resource.Instantiate("MapLocationService").GetComponent<MapLocationService>();
        MapLocationService.GetLocation(out LocationStatus, out latitude, out longitude, out altitude);

        polatedValue = 0.025f;
        navMode = false;
        mute = false;

        Managers.UI.OpenPopup<MainPopup>();
        Managers.Sound.Play(SoundID.MainBGM);
    }

    private void OnDestroy()
        => MapLocationService.StopLocationService();
}
