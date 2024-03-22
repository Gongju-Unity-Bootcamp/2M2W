using Microsoft.Geospatial;

public enum BingRouteMode
{
    Walking,
    Driving
}

public static class BingMap
{
    public const string url = "https://dev.virtualearth.net/REST/v1/";

    public static string GetDrivingUrl(LatLon originLatLon, LatLon destinationLatLon)
        => $"{url}Routes/Driving?o=json&wp.0={originLatLon.LatitudeInDegrees},{originLatLon.LongitudeInDegrees}&wp.1={destinationLatLon.LatitudeInDegrees},{destinationLatLon.LongitudeInDegrees}&key={Managers.App.MapSession.DeveloperKey}&c=ko-KR";
    
    public static string GetWalkingUrl(LatLon originLatLon, LatLon destinationLatLon)
        => $"{url}Routes/Walking?o=json&wp.0={originLatLon.LatitudeInDegrees},{originLatLon.LongitudeInDegrees}&wp.1={destinationLatLon.LatitudeInDegrees},{destinationLatLon.LongitudeInDegrees}&key={Managers.App.MapSession.DeveloperKey}&c=ko-KR";
    
    public static string GetLatLonUrl(string addressName)
        => $"{url}Locations/{addressName}?o=json&key={Managers.App.MapSession.DeveloperKey}&c=ko-KR";

    public static string GetAddressUrl(LatLon latLon)
        => $"{url}Locations/{latLon.LatitudeInDegrees, 6},{latLon.LongitudeInDegrees, 0}?key={Managers.App.MapSession.DeveloperKey}&c=ko-KR";

    public static string GetSearchAddressUrl(string addressName)
        => $"{url}Locations?query={addressName}&key={Managers.App.MapSession.DeveloperKey}&c=ko-KR";
}
