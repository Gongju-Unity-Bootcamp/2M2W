public enum StoryboardID
{
    LoadUI = 0,
    TitleUI = 1
}

public class StoryboardData
{
    public StoryboardID Id { get; set; }
    public string Prefab { get; set; }
}
