using Microsoft.Geospatial;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ArNavPlayPopup : UIPopup
{
    private enum Images
    {
        Mask,
        ScrollView
    }

    private enum RawImages
    {
        XRImage
    }

    private enum Buttons
    {
        Button_01b,
        Button_02b,
        Button_03b,
        Button_04b,

        ScreenshotButton,
        RecordButton,

        BackButton
    }

    public MarkerData data;
    private ScrollRect rect;
    private bool isRecord, playEffect;

    public override void Init()
    {
        base.Init();

        BindImage(typeof(Images));
        BindRawImage(typeof(RawImages));
        BindButton(typeof(Buttons));

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }

        LatLon latLon = Managers.App.MapLocationService.GetLatLon();
        if (latLon != default)
        {
            GetRawImage((int)RawImages.XRImage).texture = Managers.Resource.LoadRenderTexture("XRCamera");
        }
        else
        {
            GetRawImage((int)RawImages.XRImage).texture = Managers.Resource.LoadSprite("spr_DownloadBackground").texture;
        }

        rect = GetComponentInChildren<ScrollRect>();
        rect.onValueChanged.AddListener(OnDrag);
    }

    private void OnDrag(Vector2 vector)
    {
        if (true == isRecord)
        {
            isRecord = false;
            Managers.Sound.Play(SoundID.RecordEnd);
            GetButton((int)Buttons.RecordButton).image.sprite = Managers.Resource.LoadSprite("spr_RecordStart");
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
            case Buttons.Button_03b:
                Managers.UI.OpenPopup<ArNavPopup>();
                break;
            case Buttons.Button_04b:
                Managers.UI.OpenPopup<ArRoadViewPopup>();
                break;
            case Buttons.ScreenshotButton:
                if (false == isRecord)
                {
                    playEffect = true;
                    Managers.Sound.Play(SoundID.Screenshot);
                    RectTransform rectScreenshot = GetImage((int)Images.Mask).rectTransform;
                    Texture2D texture = Utilities.TakeScreenshot(data, rectScreenshot);
                }
                break;
            case Buttons.RecordButton:
                playEffect = true;
                RectTransform rectRecord = GetImage((int)Images.Mask).rectTransform;
                if (false == isRecord)
                {
                    isRecord = true;
                    Managers.Sound.Play(SoundID.RecordStart);
                    GetButton((int)button).image.sprite = Managers.Resource.LoadSprite("spr_RecordEnd");
                }
                else
                {
                    isRecord = false;
                    Managers.Sound.Play(SoundID.RecordEnd);
                    GetButton((int)button).image.sprite = Managers.Resource.LoadSprite("spr_RecordStart");
                }
                break;
            case Buttons.BackButton:
                Managers.UI.ClosePopupUI();
                break;
        }

        if (false == playEffect)
        {
            Managers.Sound.Play(SoundID.ButtonClick);
        }
        else
        {
            playEffect = false;
        }
    }
}
