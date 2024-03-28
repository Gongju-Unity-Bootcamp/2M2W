public enum SoundType
{
    None = 0,
    BGM,
    VFX
}

public enum SoundID
{
    None = 0,
    BGM = 100,
    MainBGM = 101,
    TitleBGM = 102,
    RoadBGM = 103,
    VFX = 1000,
    ButtonClick = 1001,
    ButtonBack = 1002,
    Screenshot = 1003,
    RecordStart = 1004,
    RecordEnd = 1005
}

public class SoundData
{
    public SoundID Id { get; set; }
    public string Name { get; set; }
    public float Volume { get; set; }
}
