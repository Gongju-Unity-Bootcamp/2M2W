using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ViewNavPopup : UIPopup
{
    private enum Texts
    {
        StartText,
        EndText,
    }

    private enum Buttons
    {
        CancelButton
    }

    public override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        StartCoroutine(Managers.App.startLatLon.GetAddress(response =>
        {
            LocationDetails json = JsonUtilities.JsonToObject<LocationDetails>(response);

            foreach (ResourceLocationSet resourceSet in json.resourceSets)
            {
                foreach (ResourceLocation resource in resourceSet.resources)
                {
                    if (false == string.IsNullOrEmpty(resource.address.formattedAddress))
                    {
                        GetText((int)Texts.StartText).text = resource.address.formattedAddress;
                    }
                }
            }
        }));

        StartCoroutine(Managers.App.endLatLon.GetAddress(response =>
        {
            LocationDetails json = JsonUtilities.JsonToObject<LocationDetails>(response);

            foreach (ResourceLocationSet resourceSet in json.resourceSets)
            {
                foreach (ResourceLocation resource in resourceSet.resources)
                {
                    if (false == string.IsNullOrEmpty(resource.address.formattedAddress))
                    {
                        GetText((int)Texts.EndText).text = resource.address.formattedAddress;
                    }
                }
            }
        }));

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }

        UpdateLatLon();
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
                Managers.App.updateLatLon = false;
                Managers.App.PopupPinLayer.MapPins.Remove(Managers.App.MapPin);
                Managers.UI.ClosePopupUI();
                break;
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void UpdateLatLon()
    {
        Managers.App.updateLatLon = true;
        MapPin mapPin = Managers.Resource.Instantiate("CurrentPin").GetComponent<MapPin>();
        Managers.App.MapPin = mapPin;
        LatLon latLon = Managers.App.MapLocationService.GetLatLon();
        if (latLon != default)
        {
            mapPin.Location = Managers.App.MapLocationService.GetLatLon();
        }
        else
        {
            mapPin.Location = Managers.App.startLatLon;
            mapPin.GetComponentInChildren<Text>().text = "위치 확인 불가";
        }
        mapPin.IsLayerSynchronized = false;
        Managers.App.PopupPinLayer.MapPins.Add(mapPin);
    }
}
