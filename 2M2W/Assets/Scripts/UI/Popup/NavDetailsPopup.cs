using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavDetailsPopup : UIPopup
{
    private enum Images
    {
        MaskImage
    }

    private enum Buttons
    {
        Button,

        Button_01,
        Button_02,

        Button_01b,
        Button_02b,
        Button_03b,
        Button_04b,

        BackButton
    }

    private enum Texts
    {
        Text_01,
        Text_02,
        Text_03
    }

    private ObservableList<MapPin> mapPins;

    public override void Init()
    {
        base.Init();

        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetImage((int)Images.MaskImage).sprite = Managers.Resource.LoadSprite(Managers.App.MarkerData.Path);
        SetDetailTexts();

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }

        mapPins = Managers.App.MapPinLayer.MapPins;
    }

    private void OnClickButton(PointerEventData eventData)
    {
        Buttons button = Enum.Parse<Buttons>(eventData.pointerEnter.name);

        ProcessButton(button);
    }

    private void ProcessButton(Buttons button)
    {
        mapPins.Clear();
        Managers.App.MapPinSubLayer.MapPins.Clear();
        Managers.App.MapLineRenderer.gameObject.SetActive(false);

        switch (button)
        {
            case Buttons.Button:
                LatLon latLon = Managers.App.MapLocationService.GetLatLon();
                if (latLon != default)
                {
                    Managers.App.startLatLon = latLon;
                    MapPin startPin = Managers.Resource.Instantiate("StartPin").GetComponent<MapPin>();
                    startPin.Location = Managers.App.startLatLon;
                    startPin.IsLayerSynchronized = false;
                    mapPins.Add(startPin);
                    MapPin endPin = Managers.Resource.Instantiate("EndPin").GetComponent<MapPin>();
                    endPin.Location = Managers.App.endLatLon;
                    endPin.IsLayerSynchronized = false;
                    mapPins.Add(endPin);
                    GetRenderRoute();
                    Managers.UI.OpenPopup<ViewNavPopup>();
                }
                break;
            case Buttons.Button_01:
                break;
            case Buttons.Button_02:
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

    private void SetDetailTexts()
    {
        GetText((int)Texts.Text_01).text = Managers.App.MarkerData.Name;
        GetText((int)Texts.Text_02).text = Utilities.ConvertToLocationGroupInfo(Managers.App.MarkerData.Group);
        
        StartCoroutine(new LatLon(Managers.App.MarkerData.Latitude, Managers.App.MarkerData.Longitude).GetAddress(response =>
        {
            LocationDetails json = JsonUtilities.JsonToObject<LocationDetails>(response);

            foreach (ResourceLocationSet resourceSet in json.resourceSets)
            {
                foreach (ResourceLocation resource in resourceSet.resources)
                {
                    if (false == string.IsNullOrEmpty(resource.address.formattedAddress))
                    {
                        GetText((int)Texts.Text_03).text = resource.address.formattedAddress;
                    }
                }
            }
        }));
    }

    private void GetRenderRoute()
    {
        ObservableList<MapPin> mapPins = Managers.App.MapPinSubLayer.MapPins;

        foreach (ItineraryItem itineraryItem in Managers.App.itineraryItems)
        {
            LatLon latLon = new LatLon(itineraryItem.maneuverPoint.coordinates[0], itineraryItem.maneuverPoint.coordinates[1]);
            MapPin mapPin = Managers.Resource.Instantiate("MapPin").GetComponent<MapPin>();
            mapPin.transform.Find("Root").gameObject.SetActive(false);
            mapPin.Location = latLon;
            mapPin.IsLayerSynchronized = false;
            mapPins.Add(mapPin);
        }

        Managers.App.MapLineRenderer.gameObject.SetActive(true);
        Managers.App.MapRenderer.Center = Managers.App.startLatLon;
    }
}
