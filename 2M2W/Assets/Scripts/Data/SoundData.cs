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
    VFX = 1000,
}

public class SoundData
{
    public SoundID Id { get; set; }
    public string Name { get; set; }
    public float Volume { get; set; }
}
