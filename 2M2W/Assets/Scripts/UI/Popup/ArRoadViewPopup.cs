using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Android;

public class ArRoadViewPopup : UIPopup
{
    private enum RawImages
    {
        ARImage
    }

    private enum Buttons
    {
        Button_Up,
        Button_Down,
        Button_Left,
        Button_Right,

        CurrentPosIcon,
        NavModeIcon,

        Button_01b,
        Button_02b,
        Button_03b,
        Button_04b,


        BackButton
    }

    public override void Init()
    {
        base.Init();

        BindRawImage(typeof(RawImages));
        BindButton(typeof(Buttons));

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }

        foreach (RawImages rawImageIndex in Enum.GetValues(typeof(RawImages)))
        {
            RawImage rawImage = GetRawImage((int)rawImageIndex);
            rawImage.BindViewEvent(OnDragRawImage, ViewEvent.Drag, this);
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
            case Buttons.Button_Up:
                Managers.App.MapController.PanNorth();
                break;
            case Buttons.Button_Down:
                Managers.App.MapController.PanSouth();
                break;
            case Buttons.Button_Left:
                Managers.App.MapController.PanWest();
                break;
            case Buttons.Button_Right:
                Managers.App.MapController.PanEast();
                break;
            case Buttons.CurrentPosIcon:
                LatLon latlon = Managers.App.MapLocationService.GetLatLon();
                if (latlon != default)
                {
                    Managers.App.MapRenderer.Center = latlon;
                }
                break;
            case Buttons.NavModeIcon:
                Managers.App.GetNavMode();
                break;
            case Buttons.BackButton:
                Managers.UI.ClosePopupUI();
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
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnDragRawImage(PointerEventData eventData)
    {
        Vector2 dragDelta = -eventData.delta * Managers.App.polatedValue;

        Managers.App.MapController.Pan(dragDelta, false);
    }
}

