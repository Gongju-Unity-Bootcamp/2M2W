using UnityEngine;

[RequireComponent(typeof(GameObject))]
public class Managers : MonoBehaviour
{
    public static Managers Instance;
    public static DataManager Data { get; set; }
    public static ResourceManager Resource { get; set; }
    public static UIManager UI { get; set; }
    public static SoundManager Sound { get; set; }
    public static AppManager App { get; set; }

    private void Awake()
        => Init();

    public void Init()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            GameObject gameObject;

            Data = new DataManager();
            Resource = new ResourceManager();
            UI = new UIManager();

            gameObject = new GameObject(nameof(SoundManager));
            gameObject.transform.parent = transform;
            Sound = gameObject.AddComponent<SoundManager>();

            gameObject = new GameObject(nameof(AppManager));
            gameObject.transform.parent = transform;
            App = gameObject.AddComponent<AppManager>();

            Data.Init();
            Resource.Init();
            UI.Init();
            Sound.Init();
            App.Init();
        }
    }
}
