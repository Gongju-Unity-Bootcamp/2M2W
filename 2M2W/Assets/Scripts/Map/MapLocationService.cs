using Microsoft.Geospatial;
using UnityEngine.Android;
using UnityEngine;

public class MapLocationService : MonoBehaviour
{
    public const float desiredAccuracyInMeters = 10f, updateDistanceInMeters = 1f, perSeconds = 60f;

    private LocationService locationService;

    public void StartLocationService(string permissionName = null)
    {
        if (true == Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            locationService = Input.location;
            locationService.Start(desiredAccuracyInMeters, updateDistanceInMeters);
        }
        else
        {
            PermissionCallbacks callbacks = new();
            callbacks.PermissionGranted += StartLocationService;
            Permission.RequestUserPermission(Permission.FineLocation, callbacks);
        }
    }

    public void StopLocationService()
        => locationService.Stop();

    public bool GetLocation(out LocationServiceStatus status, out double latitude, out double longitude, out double altitude)
    {
        latitude = 0d;
        longitude = 0d;
        altitude = 0d;
        status = locationService.status;

        if (!locationService.isEnabledByUser)
        {
            return false;
        }

        switch (status)
        {
            case LocationServiceStatus.Stopped:
            case LocationServiceStatus.Failed:
            case LocationServiceStatus.Initializing:
                return false;
            default:
                LocationInfo locationInfo = locationService.lastData;
                latitude = locationInfo.latitude;
                longitude = locationInfo.longitude;
                altitude = locationInfo.altitude;
                return true;
        }
    }

    public LatLon GetLatLon()
    {
        switch (locationService.status)
        {
            case LocationServiceStatus.Stopped:
            case LocationServiceStatus.Failed:
            case LocationServiceStatus.Initializing:
                return default;
            default:
                return new LatLon(locationService.lastData.latitude, locationService.lastData.longitude);
        }
    }

    private void OnDestroy()
        => StopLocationService();
}
