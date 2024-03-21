using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Microsoft.Maps.Unity;

public class MainPopup : UIPopup
{
    private enum Buttons
    {
        Button_01,
        Button_02,
        Button_03,
        Button_04,

        Button_01b,
        Button_02b,
        Button_03b,
        Button_04b,

        Button
    }

    public override void Init()
    {
        base.Init();
        
        BindButton(typeof(Buttons));

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }
    }

    private void OnClickButton(PointerEventData eventData)
    {
        Buttons button = Enum.Parse<Buttons>(eventData.pointerEnter.name);

        ProcessButton(button);
    }

    private void ProcessButton(Buttons button)
    {
        switch (button)
        {
            case Buttons.Button_01:
                Managers.UI.OpenPopup<NavPopup>();
                break;
            case Buttons.Button_02:
                Managers.UI.OpenPopup<ArNavPopup>();
                break;
            case Buttons.Button_03:
                Managers.UI.OpenPopup<ArRoadViewPopup>();
                break;
            case Buttons.Button_04:
                Application.Quit();
                break;
            case Buttons.Button_01b:
                Managers.UI.CloseAllPopupUI();
                Managers.UI.OpenPopup<MainPopup>();
                break;
            case Buttons.Button_02b:
                Managers.UI.OpenPopup<NavPopup>();
                break;
            case Buttons.Button_03b:
                Managers.UI.OpenPopup<ArNavPopup>();
                break;
            case Buttons.Button_04b:
                Managers.UI.OpenPopup<ArRoadViewPopup>();
                break;
            case Buttons.Button:
                if (false == Managers.App.mute)
                {
                    Managers.App.mute = true;
                    Managers.Sound.Pause(SoundType.BGM);
                    GetButton((int)Buttons.Button).image.sprite = Managers.Resource.LoadSprite("spr_MuteMusic");
                }
                else
                {
                    Managers.App.mute = false;
                    Managers.Sound.UnPause(SoundType.BGM);
                    GetButton((int)Buttons.Button).image.sprite = Managers.Resource.LoadSprite("spr_PlayMusic");
                }
                break;
        };

        Managers.Sound.Play(SoundID.ButtonClick);
    }
}
