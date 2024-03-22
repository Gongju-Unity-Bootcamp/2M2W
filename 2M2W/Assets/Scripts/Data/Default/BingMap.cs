using Microsoft.Geospatial;

public enum BingRouteMode
{
    Walking,
    Driving
}

public static class BingMap
{
    public const string url = "https://dev.virtualearth.net/REST/v1/";

    public static string GetDrivingUrl(LatLon originLanLot, LatLon destinationLanLot)
        => $"{url}Routes/Driving?o=json&wp.0={originLanLot.LatitudeInDegrees},{originLanLot.LongitudeInDegrees}&wp.1={destinationLanLot.LatitudeInDegrees},{destinationLanLot.LongitudeInDegrees}&key={Managers.App.MapSession.DeveloperKey}";
    public static string GetWalkingUrl(LatLon originLanLot, LatLon destinationLanLot)
        => $"{url}Routes/Walking?o=json&wp.0={originLanLot.LatitudeInDegrees},{originLanLot.LongitudeInDegrees}&wp.1={destinationLanLot.LatitudeInDegrees},{destinationLanLot.LongitudeInDegrees}&key={Managers.App.MapSession.DeveloperKey}";
    public static string GetLatLonUrl(string addressName)
        => $"{url}Locations/{addressName}?o=json&key={Managers.App.MapSession.DeveloperKey}";
}
