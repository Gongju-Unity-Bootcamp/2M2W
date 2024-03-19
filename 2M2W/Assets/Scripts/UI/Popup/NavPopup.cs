using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class NavPopup : UIPopup
{
    private enum RawImages
    {
        RawImage
    }

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

        BackButton
    }

    private MapRenderer map;
    private LatLon lastLatLon;

    public override void Init()
    {
        base.Init();

        BindRawImage(typeof(RawImages));
        BindButton(typeof(Buttons));

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
            button.BindViewEvent(OnDragButton, ViewEvent.Drag, this);
        }

        foreach (RawImages rawImageIndex in Enum.GetValues(typeof(RawImages)))
        {
            RawImage rawImage = GetRawImage((int)rawImageIndex);
            rawImage.BindViewEvent(OnBeginDragRawImage, ViewEvent.BeginDrag, this);
            rawImage.BindViewEvent(OnDragRawImage, ViewEvent.Drag, this);
            rawImage.BindViewEvent(OnDoubleClickRawImage, ViewEvent.DoubleClick, this);
        }

        map = Managers.App.MapRenderer;
    }

    private void OnClickButton(PointerEventData eventData)
    {
        Buttons button = Enum.Parse<Buttons>(eventData.pointerEnter.name);

        ProcessButton(button);
    }

    private void OnDragButton(PointerEventData eventData)
    {
        Button button = eventData.pointerEnter.GetComponent<Button>();
        RectTransform buttonGroup = button.GetComponentInParent<HorizontalLayoutGroup>().GetComponent<RectTransform>();

        Vector2 delta = eventData.delta;
        float deltaX = buttonGroup.InverseTransformDirection(delta).x;
        buttonGroup.localPosition += new Vector3(deltaX, 0f, 0f);
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

    private void OnClickNavButton()
    {
        Managers.UI.OpenPopup<NavPopup>();

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnClickArNavButton()
    {
        Managers.UI.OpenPopup<ArNavPopup>();

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnClickArRoadViewButton()
    {
        Managers.UI.OpenPopup<ArRoadViewPopup>();

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnClickHomeButton()
    {
        Managers.UI.CloseAllPopupUI();

        Managers.UI.OpenPopup<MainPopup>();

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnBackButton()
    {
        Managers.UI.ClosePopupUI();

        Managers.Sound.Play(SoundID.ButtonBack);
    }

    private void OnBeginDragRawImage(PointerEventData eventData)
    {
        if (map.Raycast(GetRay(eventData), out MapRendererRaycastHit hitInfo))
        {
            lastLatLon = new LatLon(hitInfo.Location.LatitudeInDegrees, hitInfo.Location.LongitudeInDegrees);
        }
    }

    private void OnDragRawImage(PointerEventData eventData)
    {
        if (map.Raycast(GetRay(eventData), out MapRendererRaycastHit hitInfo))
        {
            double latDelta = (hitInfo.Location.LatitudeInDegrees - lastLatLon.LatitudeInDegrees) * Managers.App.polatedValue;
            double lonDelta = (hitInfo.Location.LongitudeInDegrees - lastLatLon.LongitudeInDegrees) * Managers.App.polatedValue;

            LatLon newLatLon = new LatLon(map.Center.LatitudeInDegrees - latDelta, map.Center.LongitudeInDegrees - lonDelta);

            map.SetMapScene(new MapSceneOfLocationAndZoomLevel(newLatLon, map.ZoomLevel), MapSceneAnimationKind.None);
        }
    }

    private void OnDoubleClickRawImage(PointerEventData eventData)
    {
        if (map.Raycast(GetRay(eventData), out MapRendererRaycastHit hitInfo))
        {
            LatLon latLon = new LatLon(hitInfo.Location.LatitudeInDegrees, hitInfo.Location.LongitudeInDegrees);
        }
    }

    private Ray GetRay(PointerEventData eventData)
    {
        RawImage rawImage = eventData.pointerEnter.GetComponent<RawImage>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage.rectTransform, Input.mousePosition, null, out Vector2 localPoint);

        return Camera.main.ScreenPointToRay(localPoint);
    }
}
