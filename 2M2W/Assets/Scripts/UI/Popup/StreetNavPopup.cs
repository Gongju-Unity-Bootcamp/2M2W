using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class StreetNavPopup : UIPopup
{
    private enum Objects
    {
        TextNav
    }

    private enum Texts
    {
        SearchText,

        Text_01,
        Text_02,
        Text_03
    }

    private enum Buttons
    {
        SearchButton,

        Button_01,
        Button_02, 
        Button_03, 
        Button_04,

        CurrentPosIcon,
        CancelButton,

        Button,

        NavStart
    }

    private enum InputFields
    {
        InputField_01,
        InputField_02
    }

    private GameObject obj;
    private Button searchButton;
    private TMP_Text text;
    private TMP_InputField input;
    private TMP_InputField[] inputs;
    private ObservableList<MapPin> mapPins;

    public override void Init()
    {
        base.Init();

        BindObject(typeof(Objects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindInputField(typeof(InputFields));

        obj = GetObject((int)Objects.TextNav);
        obj.SetActive(false);
        text = GetText((int)Texts.SearchText);

        searchButton = GetButton((int)Buttons.SearchButton);
        searchButton.gameObject.SetActive(false);

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }

        inputs = new TMP_InputField[Enum.GetValues(typeof(InputFields)).Length];

        foreach (InputFields inputFieldIndex in Enum.GetValues(typeof(InputFields)))
        {
            TMP_InputField inputField = GetInputField((int)inputFieldIndex);
            inputField.onSelect.AddListener(_ =>
            {
                input = inputField;
                text.text = input.text;
                OnChangedInputField(text.text, inputField);

                searchButton.gameObject.SetActive(true);
            });
            inputField.onValueChanged.AddListener(text => { OnChangedInputField(text, inputField); });
            inputs[(int)inputFieldIndex] = inputField;
            if (inputFieldIndex == 0)
            {
                LatLon latLon = Managers.App.MapLocationService.GetLatLon();
                Managers.App.startLatLon = latLon;

                StartCoroutine(latLon.GetAddress(response =>
                {
                    LocationDetails json = JsonUtilities.JsonToObject<LocationDetails>(response);

                    foreach (ResourceLocationSet resourceSet in json.resourceSets)
                    {
                        foreach (ResourceLocation resource in resourceSet.resources)
                        {
                            if (false == string.IsNullOrEmpty(resource.address.formattedAddress))
                            {
                                input = inputField;
                                text.text = string.Empty;
                                inputField.text = resource.address.formattedAddress;
                            }
                        }
                    }
                }));
            }
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
            case Buttons.SearchButton:
                TMP_Text text = GetText((int)Texts.SearchText);

                if (false == string.IsNullOrEmpty(text.text))
                {
                    input.text = text.text;
                }
                break;
            case Buttons.Button_01:
                GetFindLocation(BingRouteMode.Driving);
                break;
            case Buttons.Button_02:
                GetFindLocation(BingRouteMode.Walking);
                break;
            case Buttons.Button_03:
                GetFindLocation(BingRouteMode.Transit);
                break;
            case Buttons.Button_04:
                GetFindLocation(BingRouteMode.Bicycling);
                break;
            case Buttons.CurrentPosIcon:
                LatLon latlon = Managers.App.MapLocationService.GetLatLon();
                if (latlon != default)
                {
                    Managers.App.MapRenderer.Center = latlon;
                }
                break;
            case Buttons.CancelButton:
                Managers.UI.ClosePopupUI();
                break;
            case Buttons.Button:
                string str = inputs[0].text;
                inputs[0].text = inputs[1].text;
                inputs[1].text = str;
                break;
            case Buttons.NavStart:
                MapPin startPin = Managers.Resource.Instantiate("StartPin").GetComponent<MapPin>();
                startPin.Location = Managers.App.startLatLon;
                mapPins.Add(startPin);
                MapPin endPin = Managers.Resource.Instantiate("EndPin").GetComponent<MapPin>();
                endPin.Location = Managers.App.endLatLon;
                mapPins.Add(endPin);
                Managers.App.MapRenderer.Center = Managers.App.startLatLon;
                GetRenderRoute();
                break;
        }

        searchButton.gameObject.SetActive(false);
        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnChangedInputField(string addressName, TMP_InputField inputField)
        => GetRoute(addressName, inputField);

    private void GetFindLocation(BingRouteMode bingRouteMode)
    {
        obj.SetActive(false);

        foreach (TMP_InputField inputField in inputs)
        {
            if (true == string.IsNullOrEmpty(inputField.text))
            {
                return;
            }
        }

        StartCoroutine(Managers.App.startLatLon.GetRoute(Managers.App.endLatLon, response =>
        {
            if (Managers.App.startLatLon == Managers.App.endLatLon)
            {
                return;
            }

            RouteDetails json = JsonUtilities.JsonToObject<RouteDetails>(response);

            foreach (ResourceRouteSet resourceSet in json.resourceSets)
            {
                foreach (ResourceRoute resourceRoute in resourceSet.resources)
                {
                    foreach (RouteLeg routeLeg in resourceRoute.routeLegs)
                    {
                        if (false == string.IsNullOrEmpty(routeLeg.travelMode))
                        {
                            GetText((int)Texts.Text_01).text = $"{routeLeg.travelDuration} {resourceRoute.durationUnit}";
                            GetText((int)Texts.Text_02).text = $"{routeLeg.travelDistance} {resourceRoute.distanceUnit}";
                            GetText((int)Texts.Text_03).text = $"{routeLeg.travelMode}";
                            obj.SetActive(true);

                            Managers.App.itineraryItems = routeLeg.itineraryItems;
                        }
                    }
                }
            }
        }, bingRouteMode));
    }

    private void GetRoute(string str, TMP_InputField inputField)
    {
        input = inputField;

        StartCoroutine(str.GetSearchAddress(response =>
        {
            LocationDetails json = JsonUtilities.JsonToObject<LocationDetails>(response);

            foreach (ResourceLocationSet resourceSet in json.resourceSets)
            {
                foreach (ResourceLocation resource in resourceSet.resources)
                {
                    if (false == string.IsNullOrEmpty(resource.address.formattedAddress))
                    {
                        text.text = resource.address.formattedAddress;

                        foreach (GeocodePoint geocode in resource.geocodePoints)
                        {
                            if (inputField == inputs[0])
                            {
                                Managers.App.startLatLon = new LatLon(geocode.coordinates[0], geocode.coordinates[1]);
                            }
                            else
                            {
                                Managers.App.endLatLon = new LatLon(geocode.coordinates[0], geocode.coordinates[1]);
                            }
                        }
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
            mapPins.Add(mapPin);
        }

        Managers.App.MapLineRenderer.gameObject.SetActive(true);
    }
}
