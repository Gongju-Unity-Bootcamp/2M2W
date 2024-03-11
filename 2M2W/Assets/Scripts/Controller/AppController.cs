using UnityEngine;

public class AppController : MonoBehaviour
{
    private void Awake()
        => Init();

    private void Init()
    {
        Managers.UI.OpenPopup<MainPopup>();
    }
}
