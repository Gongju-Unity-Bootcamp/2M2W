using System;
using System.Collections.Generic;

[System.Serializable]
public class RouteDetails
{
    public string authenticationResultCode;
    public string brandLogoUri;
    public string copyright;
    public ResourceRouteSet[] resourceSets;
    public int statusCode;
    public string statusDescription;
    public string traceId;
}

[System.Serializable]
public class ResourceRouteSet
{
    public int estimatedTotal;
    public ResourceRoute[] resources;
}

[System.Serializable]
public class ResourceRoute
{
    public string __type;
    public double[] bbox;
    public string id;
    public string distanceUnit;
    public string durationUnit;
    public RouteLeg[] routeLegs;
    public string trafficCongestion;
    public string trafficDataUsed;
    public double travelDistance;
    public int travelDuration;
    public int travelDurationTraffic;
    public string travelMode;
}

[System.Serializable]
public class RouteLeg
{
    public RouteWaypoint actualEnd;
    public RouteWaypoint actualStart;
    public ItineraryItem[] itineraryItems;
    public string routeRegion;
    public RouteSubLeg[] routeSubLegs;
    public DateTime startTime;
    public double travelDistance;
    public int travelDuration;
    public string travelMode;
}

[System.Serializable]
public class RouteWaypoint
{
    public Point maneuverPoint;
    public string sideOfStreet;
}

[System.Serializable]
public class ItineraryItem
{
    public ItineraryItemDetails[] details;
    public string iconType;
    public ItineraryItemInstruction instruction;
    public bool isRealTimeTransit;
    public Point maneuverPoint;
    public int realTimeTransitDelay;
    public string sideOfStreet;
    public double travelDistance;
    public int travelDuration;
    public string travelMode;
}

[System.Serializable]
public class ItineraryItemDetails
{
    public int[] endPathIndices;
    public string maneuverType;
    public string mode;
    public string[] names;
    public string roadType;
    public int[] startPathIndices;
}

[System.Serializable]
public class ItineraryItemInstruction
{
    public string formattedText;
    public string maneuverType;
    public string text;
}

[System.Serializable]
public class RouteSubLeg
{
    public RouteWaypoint endWaypoint;
    public RouteWaypoint startWaypoint;
    public double travelDistance;
    public int travelDuration;
}

[System.Serializable]
public class Point
{
    public string type;
    public double[] coordinates;
}

[System.Serializable]
public class Address
{
    public string addressLine;
    public string adminDistrict;
    public string countryRegion;
    public string formattedAddress;
    public Dictionary<string, object> intersection;
    public string locality;
    public string postalCode;
}

[System.Serializable]
public class GeocodePoint
{
    public string type;
    public double[] coordinates;
    public string calculationMethod;
    public string[] usageTypes;
}

[System.Serializable]
public class ResourceLocation
{
    public string __type;
    public double[] bbox;
    public string name;
    public Point point;
    public Address address;
    public string confidence;
    public string entityType;
    public GeocodePoint[] geocodePoints;
    public string[] matchCodes;
}

[System.Serializable]
public class ResourceLocationSet
{
    public int estimatedTotal;
    public ResourceLocation[] resources;
}

[System.Serializable]
public class LocationDetails
{
    public string authenticationResultCode;
    public string brandLogoUri;
    public string copyright;
    public ResourceLocationSet[] resourceSets;
    public int statusCode;
    public string statusDescription;
    public string traceId;
}