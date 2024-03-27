using Microsoft.Geospatial;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArNavPlayPopup : UIPopup
{
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
    
        BackButton
    }

    public MarkerData data;
    
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

        LatLon latLon = Managers.App.MapLocationService.GetLatLon();
        if (latLon != default)
        {
            GetRawImage((int)RawImages.XRImage).texture = Managers.Resource.LoadRenderTexture("XRCamera");
        }
        else
        {
            GetRawImage((int)RawImages.XRImage).texture = Managers.Resource.LoadSprite("spr_DownloadBackground").texture;
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
            case Buttons.BackButton:
                Managers.UI.ClosePopupUI();
                break;
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }
}
