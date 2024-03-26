using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArNavPopup : UIPopup
{
    private enum Objects
    {
        Content
    }

    private enum Buttons
    {
        Button_01b,
        Button_02b,
        Button_03b,
        Button_04b,

        BackButton
    }

    private List<UISubItem> subItems;
    private Transform obj;

    public override void Init()
    {
        base.Init();

        BindObject(typeof(Objects));
        BindButton(typeof(Buttons));

        foreach (Objects objectIndex in Enum.GetValues(typeof(Objects)))
        {
            obj = GetObject((int)objectIndex).transform;
        }

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }

        subItems = new List<UISubItem>();

        foreach (MarkerData data in Managers.App.CameraService.docents)
        {
            ArNavSubItem subItem = Managers.UI.OpenSubItem<ArNavSubItem>(obj);
            subItem.data = data;
            subItems.Add(subItem);
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
            case Buttons.Button_01b:
                Managers.UI.CloseAllPopupUI();
                Managers.UI.OpenPopup<MainPopup>();
                break;
            case Buttons.Button_02b:
                Managers.UI.OpenPopup<NavPopup>();
                break;
            case Buttons.Button_04b:
                Managers.UI.OpenPopup<ArRoadViewPopup>();
                break;
            case Buttons.BackButton:
                Managers.UI.ClosePopupUI();
                break;
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnDestroy()
    {
        foreach (UISubItem subItem in subItems)
        {
            subItem.CloseSubItem();
        }
    }
}
