using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class NavPopup : UIPopup
{
    private enum RawImages
    {
        RawImage
    }

    private enum Buttons
    {
        NavModeIcon,
        CurrentPosIcon,
        EnlargementIcon,

        Button_01, 
        Button_02, 
        Button_03, 
        Button_04,
        Button_05,
        Button_06,
        Button_07,
        Button_08,

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
            rawImage.BindViewEvent(OnClickRawImage, ViewEvent.Click, this);
            rawImage.BindViewEvent(OnDoubleClickRawImage, ViewEvent.DoubleClick, this);
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
            case Buttons.NavModeIcon:
                Managers.App.SetNavMode();
                break;
            case Buttons.CurrentPosIcon:
                LatLon latLon = Managers.App.MapLocationService.GetLatLon();
                if (latLon != default)
                {
                    Managers.App.MapRenderer.Center = latLon;
                }
                break;
            case Buttons.EnlargementIcon:
                Managers.UI.OpenPopup<StreetNavPopup>();
                break;
            case Buttons.Button_01:
                Managers.App.EnableMarkerPin(0);
                break;
            case Buttons.Button_02:
                Managers.App.EnableMarkerPin(1);
                break;
            case Buttons.Button_03:
                Managers.App.EnableMarkerPin(2);
                break;
            case Buttons.Button_04:
                Managers.App.EnableMarkerPin(3);
                break;
            case Buttons.Button_05:
                Managers.App.EnableMarkerPin(4);
                break;
            case Buttons.Button_06:
                Managers.App.EnableMarkerPin(5);
                break;
            case Buttons.Button_07:
                Managers.App.EnableMarkerPin(6);
                break;
            case Buttons.Button_08:
                Managers.App.EnableMarkerPin(7);
                break;
            case Buttons.Button_01b:
                Managers.UI.CloseAllPopupUI();
                Managers.UI.OpenPopup<MainPopup>();
                break;
            case Buttons.Button_03b:
                Managers.UI.OpenPopup<ArNavPopup>();
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

    private void OnDragRawImage(PointerEventData eventData)
    {
        Vector2 dragDelta = -eventData.delta * Managers.App.polatedValue;

        Managers.App.MapController.Pan(dragDelta, false);
    }

    private void OnClickRawImage(PointerEventData eventData)
    {
        Ray ray = eventData.GetRay();

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject gameObject = hit.collider.GetComponentInChildren<Button>().gameObject;

            if (gameObject != null)
            {
                ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerClickHandler);
            }
        }
    }

    private void OnDoubleClickRawImage(PointerEventData eventData)
    {
        if (Managers.App.MapRenderer.Raycast(eventData.GetRay(), out MapRendererRaycastHit hitInfo))
        {
            LatLon latLon = new LatLon(hitInfo.Location.LatitudeInDegrees, hitInfo.Location.LongitudeInDegrees);
            ObservableList<MapPin> mapPins = Managers.App.PopupPinLayer.MapPins;

            if (mapPins.Count > 0)
            {
                mapPins.Clear();
            }
            else
            {
                MapPin mapPin = Managers.Resource.Instantiate("MapPin").GetComponent<MapPin>();
                mapPin.Location = latLon;
                mapPin.IsLayerSynchronized = false;
                mapPins.Add(mapPin);
            }
        }
    }
}
