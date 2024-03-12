using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class NavPopup : UIPopup
{
    private enum Buttons
    {
        Button_01, 
        Button_02, 
        Button_03, 
        Button_04, 
        Button_05, 
        Button_06, 
        Button_07,

        Button_01b,
        Button_02b,
        Button_03b,
        Button_04b,

        BackButton,
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
            button.BindViewEvent(OnDragButton, ViewEvent.Drag, this);
        }
    }

    private void OnClickButton(PointerEventData eventData)
    {
        Buttons button = Enum.Parse<Buttons>(eventData.pointerEnter.name);

        ProcessButton(button);
    }

    private void OnDragButton(PointerEventData eventData)
    {
        Buttons button = Enum.Parse<Buttons>(eventData.pointerEnter.name);

        ScrollButton(button);
    }

    private void ProcessButton(Buttons button)
    {
        switch (button)
        {
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
            case Buttons.BackButton:
                OnBackButton();
                break;
        }
    }

    private void ScrollButton(Buttons button)
    {

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

    private void OnClickHomeButton()
    {
        Managers.UI.CloseAllPopupUI();

        Managers.UI.OpenPopup<MainPopup>();
    }

    private void OnBackButton()
    {
        Managers.UI.ClosePopupUI();
    }
}
