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
        ARImage,
        RawImage
    }

    private enum Buttons
    {
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
            case Buttons.Button_Left:
                Managers.App.cameraPivot.transform.Rotate(Vector3.down * 30f);
                break;
            case Buttons.Button_Right:
                Managers.App.cameraPivot.transform.Rotate(Vector3.up * 30f);
                break;
            case Buttons.CurrentPosIcon:
                if (true == Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    Managers.App.MapRenderer.Center = Managers.App.MapLocationService.GetLatLon();
                }
                else
                {
                    Managers.App.MapRenderer.Center = new LatLon(36.5212388346086, 127.172650559606);
                }
                break;
            case Buttons.NavModeIcon:
                if (false == Managers.App.navMode)
                {
                    Managers.App.navMode = true;
                    Managers.App.NavTile.ImageryType = MapImageryType.Symbolic;
                    Managers.App.NavTile.ImageryStyle = MapImageryStyle.Vibrant;
                }
                else
                {
                    Managers.App.navMode = false;
                    Managers.App.NavTile.ImageryType = MapImageryType.Aerial;
                }
                break;
            case Buttons.BackButton:
                Managers.UI.ClosePopupUI();
                break;
            case Buttons.Button_01b:
                Managers.UI.CloseAllPopupUI();
                Managers.UI.OpenPopup<MainPopup>();
                break;
            case Buttons.Button_02b:
                Managers.App.MapRenderer.MapTerrainType = MapTerrainType.Flat;
                Managers.UI.OpenPopup<NavPopup>();
                break;
            case Buttons.Button_03b:
                Managers.UI.OpenPopup<ArNavPopup>();
                break;
            case Buttons.Button_04b:
                Managers.App.MapRenderer.MapTerrainType = MapTerrainType.Elevated;
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

