using UnityEngine;

public class AppManager : MonoBehaviour
{
    public void Init()
    {
        Managers.UI.OpenPopup<MainPopup>();

        Managers.Sound.Play(SoundID.MainBGM);
    }
}
