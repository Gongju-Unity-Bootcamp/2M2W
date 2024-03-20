using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Android;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [HideInInspector] public XROrigin XROrigin;
    [HideInInspector] public XRInteractionManager XRManager;

    [HideInInspector] public GameObject BingMap;

    [HideInInspector] public MapSession MapSession;
    [HideInInspector] public MapRenderer MapRenderer;
    [HideInInspector] public MapInteractionController MapController;
    [HideInInspector] public DefaultTextureTileLayer NavTile;

    [HideInInspector] public LatLon latLon;
    [HideInInspector] public float polatedValue;
    [HideInInspector] public bool navMode;
    [HideInInspector] public bool mute;

    public void Init() 
    {
        XROrigin = Managers.Resource.Instantiate("XROrigin").GetComponent<XROrigin>();
        XRManager = Managers.Resource.Instantiate("XRManager").GetComponent<XRInteractionManager>();

        BingMap = Managers.Resource.Instantiate("BingMap");

        MapSession = BingMap.GetComponent<MapSession>();
        MapRenderer = BingMap.GetComponent<MapRenderer>();
        MapController = BingMap.GetComponent<MapInteractionController>();
        NavTile = BingMap.GetComponent<DefaultTextureTileLayer>();

        polatedValue = 0.025f;
        navMode = false;
        mute = false;

        Managers.UI.OpenPopup<MainPopup>();
        Managers.Sound.Play(SoundID.MainBGM);

        StartCoroutine(GetLatLonPermissionCo());
    }

    public IEnumerator GetLatLonPermissionCo()
    {
        if (false == Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            while (false == Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }
        }

        if (false == Input.location.isEnabledByUser) { yield break; }

        Input.location.Start();
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);

            --maxWait;
        }

        if (maxWait <= 0) { yield break; }
        if (Input.location.status == LocationServiceStatus.Failed) { yield break; }
        else
        {
            latLon = new LatLon(Input.location.lastData.latitude, Input.location.lastData.longitude);
        }

        Input.location.Stop();
    }
}
