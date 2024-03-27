using Microsoft.Geospatial;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ArNavDetailsPopup : UIPopup
{
    private enum Images
    {
        DocentImage
    }

    private enum Texts
    {
        Text_01,
        Text_02
    }

    private enum Buttons
    {
        Button_01,
        Button_02,

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

        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        Texture2D texture = Managers.Resource.LoadTexture2D(data.Ref);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        GetImage((int)Images.DocentImage).sprite = sprite;

        GetText((int)Texts.Text_01).text = data.Name;
        GetText((int)Texts.Text_02).text = data.Desc;

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
                Managers.App.MapRenderer.Center = new LatLon(data.Latitude, data.Longitude);
                Managers.UI.OpenPopup<StreetNavPopup>();
                break;
            case Buttons.Button_02:
                ArNavPlayPopup popup = Managers.UI.OpenPopup<ArNavPlayPopup>();
                popup.data = data;
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
            case Buttons.BackButton:
                Managers.UI.ClosePopupUI();
                break;
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }
}
