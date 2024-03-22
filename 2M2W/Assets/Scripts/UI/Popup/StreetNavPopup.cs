using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class StreetNavPopup : UIPopup
{
    private enum Buttons
    {
        Button_01,
        Button_02, 
        Button_03, 
        Button_04,

        CancelButton,

        Button
    }

    private enum Texts
    {
        Text
    }

    private enum InputFields
    {
        InputField
    }

    public override void Init()
    {
        base.Init();

        BindButton(typeof(Buttons));
        BindInputField(typeof(InputFields));

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }

        foreach (Texts textIndex in Enum.GetValues(typeof(Texts)))
        {
            TMP_Text text = GetText((int)textIndex);
            text.BindViewEvent(OnEnableText, ViewEvent.Click, this);
            text.gameObject.SetActive(true);
        }

        foreach (InputFields inputFieldIndex in Enum.GetValues(typeof(InputFields)))
        {
            TMP_InputField inputField = GetInputField((int)inputFieldIndex);
            inputField.BindViewEvent(OnClickInputField, ViewEvent.Click, this);
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
            case Buttons.CancelButton:
                Managers.UI.ClosePopupUI();
                break;
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnEnableText(PointerEventData eventData)
    {

    }

    private void OnClickInputField(PointerEventData eventData)
    {
        TMP_InputField inputField = eventData.pointerEnter.GetComponent<TMP_InputField>();

    }
}
