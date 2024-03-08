public enum StoryboardID
{
    None = 0,
    LoadUI,
    TitleUI
}

public class StoryboardData
{
    public StoryboardID Id { get; set; }
    public string Prefab { get; set; }
}
