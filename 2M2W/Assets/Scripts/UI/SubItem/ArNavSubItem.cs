using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ArNavSubItem : UISubItem
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
        Button
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
            case Buttons.Button:
                ArNavDetailsPopup popup = Managers.UI.OpenPopup<ArNavDetailsPopup>();
                popup.data = data;
                break;
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }
}
