using Microsoft.Geospatial;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StreetNavPopup : UIPopup
{
    private enum Objects
    {
        TextNav
    }

    private enum Texts
    {
        Search,
        Text_01,
        Text_02,
        Text_03
    }

    private enum Buttons
    {
        Button_01,
        Button_02, 
        Button_03, 
        Button_04,

        CancelButton,

        Button
    }

    private enum InputFields
    {
        InputField_01,
        InputField_02
    }

    private TMP_Text text;
    private TMP_InputField input;
    private TMP_InputField[] inputs;

    public override void Init()
    {
        base.Init();

        BindObject(typeof(Objects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindInputField(typeof(InputFields));

        GetObject((int)Objects.TextNav).gameObject.SetActive(false);

        text = GetText((int)Texts.Search);
        text.BindViewEvent(OnClickText, ViewEvent.Click, this);

        foreach (Buttons buttonIndex in Enum.GetValues(typeof(Buttons)))
        {
            Button button = GetButton((int)buttonIndex);
            button.BindViewEvent(OnClickButton, ViewEvent.Click, this);
        }

        inputs = new TMP_InputField[Enum.GetValues(typeof(InputFields)).Length];

        foreach (InputFields inputFieldIndex in Enum.GetValues(typeof(InputFields)))
        {
            TMP_InputField inputField = GetInputField((int)inputFieldIndex);
            inputField.onValueChanged.AddListener(text => { OnChangedInputField(text, inputField); });
            inputs[(int)inputFieldIndex] = inputField;
            if (inputFieldIndex == 0)
            {
                LatLon latLon = Managers.App.MapLocationService.GetLatLon();
                Managers.App.startLatLon = latLon;

                StartCoroutine(latLon.GetAddress(response =>
                {
                    if (response == null)
                    {
                        return;
                    }

                    LocationDetails json = JsonUtilities.JsonToObject<LocationDetails>(response);

                    foreach (ResourceLocationSet resourceSet in json.resourceSets)
                    {
                        foreach (ResourceLocation resource in resourceSet.resources)
                        {
                            inputField.text = resource.address.formattedAddress;
                        }
                    }
                }));
            }
        }
    }

    private void OnClickText(PointerEventData eventData)
    {
        Texts textIndex = Enum.Parse<Texts>(eventData.pointerEnter.name);
        TMP_Text text = GetText((int)textIndex);

        if (input != null)
        {
            input.text = text.text;
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
                Managers.App.BingRouteMode = BingRouteMode.Driving;
                GetFindLocation();
                break;
            case Buttons.Button_02:
                Managers.App.BingRouteMode = BingRouteMode.Walking;
                GetFindLocation();
                break;
            case Buttons.CancelButton:
                Managers.UI.ClosePopupUI();
                break;
            case Buttons.Button:
                GetFindLocation();
                break;
        }

        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnChangedInputField(string addressName, TMP_InputField inputField)
    {
        input = inputField;

        StartCoroutine(addressName.GetSearchAddress(response =>
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
                            if (input == inputs[0])
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

    private void GetFindLocation()
    {
        GetObject((int)Objects.TextNav).gameObject.SetActive(false);

        foreach (TMP_InputField inputField in inputs)
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                return;
            }
        }

        StartCoroutine(Managers.App.startLatLon.GetRoute(Managers.App.endLatLon, response =>
        {
            RouteDetails json = JsonUtilities.JsonToObject<RouteDetails>(response);

            foreach (ResourceRouteSet resourceSet in json.resourceSets)
            {
                foreach (ResourceRoute resourceRoute in resourceSet.resources)
                {
                    foreach (RouteLeg routeLeg in resourceRoute.routeLegs)
                    {
                        GetText((int)Texts.Text_01).text = $"{routeLeg.travelDuration} {resourceRoute.durationUnit}";
                        GetText((int)Texts.Text_02).text = $"{routeLeg.travelDistance} {resourceRoute.distanceUnit}";
                        GetText((int)Texts.Text_03).text = $"{routeLeg.travelMode}";
                        GetObject((int)Objects.TextNav).gameObject.SetActive(true);

                        Managers.App.itineraryItems = routeLeg.itineraryItems;
                    }
                }
            }
        }, Managers.App.BingRouteMode));
    }
}
