using UnityEngine;

[RequireComponent(typeof(GameObject))]
public class Managers : MonoBehaviour
{
    public static Managers Instance;

    public static DataManager Data { get; set; }
    public static ResourceManager Resource { get; set; }
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
            DontDestroyOnLoad(gameObject);

            Data = new DataManager();
            Resource = new ResourceManager();
            UI = new UIManager();

            Data.Init();
            Resource.Init();
            UI.Init();
        }
    }
}
