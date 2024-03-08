using UnityEngine;

[RequireComponent(typeof(GameObject))]
public class Managers : MonoBehaviour
{
    public static Managers Instance;
    public static DataManager Data { get; set; }
    public static ResourceManager Resource { get; set; }
    public static SoundManager Sound { get; set; }
    public static UIManager UI { get; set; }

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

            Data = new DataManager();
            Resource = new ResourceManager();
            UI = new UIManager();

            GameObject gameObject = new GameObject(nameof(SoundManager));
            gameObject.transform.parent = transform;
            Sound = gameObject.AddComponent<SoundManager>();

            Data.Init();
            Resource.Init();
            UI.Init();
            Sound.Init();
        }
    }
}
