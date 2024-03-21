public enum PopupID
{
    None = 0,
    MainUI,
    NavUI,
    ArNavUI
}

public class PopupData
{
    public PopupID Id { get; set; }
    public string Name { get; set; }
    public string Prefab { get; set; }
}
