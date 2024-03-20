using Microsoft.Maps.Unity;
using System.Collections;
using Unity.XR.CoreUtils;
using Microsoft.Geospatial;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.Android;

public class AppManager : MonoBehaviour
{
    [HideInInspector] public XROrigin XROrigin;
    [HideInInspector] public XRInteractionManager XRManager;

    [HideInInspector] public MapSession MapSession;
    [HideInInspector] public MapRenderer MapRenderer;

    [HideInInspector] public LatLon latLon;

    [HideInInspector] public float polatedValue = 1.5f;

    public void Init() 
    {
        XROrigin = Managers.Resource.Instantiate("XROrigin").GetComponent<XROrigin>();
        XRManager = Managers.Resource.Instantiate("XRManager").GetComponent<XRInteractionManager>();

        MapSession = Managers.Resource.Instantiate("MapSession").GetComponent<MapSession>();
        MapRenderer = Managers.Resource.Instantiate("MapRenderer").GetComponent<MapRenderer>();

        Managers.UI.OpenPopup<MainPopup>();
        Managers.Sound.Play(SoundID.MainBGM);

        StartCoroutine(GetLatLonPermissionCo());
    }

    private IEnumerator GetLatLonPermissionCo()
    {
        if (false == Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            while (false == Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }
        }

        if (false == Input.location.isEnabledByUser)
        {
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);

            --maxWait;
        }

        if (maxWait <= 0)
        {
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            yield break;
        }
        else
        {
            latLon = new LatLon(Input.location.lastData.latitude, Input.location.lastData.longitude);
        }

        Input.location.Stop();
    }
}
