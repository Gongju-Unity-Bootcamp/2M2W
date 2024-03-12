using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MainPopup : UIPopup
{
    private enum Buttons
    {
        Button_01,
        Button_02,
        Button_03,
        Button_04,

        EnlargementIcon,

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
                OnClickNavButton();
                break;
            case Buttons.Button_02:
                OnClickArNavButton();
                break;
            case Buttons.Button_03:
                OnClickArRoadViewButton();
                break;
            case Buttons.Button_04:
                OnClickQuitButton();
                break;
            case Buttons.Button_01b:
                OnClickHomeButton();
                break;
            case Buttons.Button_02b:
                OnClickNavButton();
                break;
            case Buttons.Button_03b:
                OnClickArNavButton();
                break;
            case Buttons.Button_04b:
                OnClickArRoadViewButton();
                break;
        };
    }

    private void OnClickNavButton()
    {
        Managers.UI.OpenPopup<NavPopup>();
    }

    private void OnClickArNavButton()
    {
        Managers.UI.OpenPopup<ArNavPopup>();
    }

    private void OnClickArRoadViewButton()
    {
        Managers.UI.OpenPopup<ArRoadViewPopup>();
    }

    private void OnClickQuitButton()
    {
        Application.Quit();
    }

    private void OnClickHomeButton()
    {
        Managers.UI.CloseAllPopupUI();

        Managers.UI.OpenPopup<MainPopup>();
    }
}
