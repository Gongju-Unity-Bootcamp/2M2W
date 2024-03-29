public enum MarkerID
{
    None = 0,
}

public class MarkerData
{
    public MarkerID Id { get; set; }
    public string Name { get; set; }
    public int Group { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Path { get; set; }
    public string Link { get; set; }
    public int Coupon { get; set; }
    public string Ref { get; set; }
    public string Desc { get; set; }
}