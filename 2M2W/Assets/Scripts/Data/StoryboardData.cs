public enum StoryboardID
{
    None = 0,
    MainUI,
    NavUI,
    ArNavUI
}

public class StoryboardData
{
    public StoryboardID Id { get; set; }
    public string Name { get; set; }
    public string Prefab { get; set; }
}
