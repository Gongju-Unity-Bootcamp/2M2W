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

    private enum RawImages
    {
        RawImage
    }

    private enum Buttons
    {
        SearchButton,

        Button_01,
        Button_02, 
        Button_03, 
        Button_04,

        NavModeIcon,
        CurrentPosIcon,
        PlusIcon,
        MinusIcon,
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
        BindRawImage(typeof(RawImages));
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

        GetRawImage((int)RawImages.RawImage).BindViewEvent(OnDragRawImage, ViewEvent.Drag, this);
        GetRawImage((int)RawImages.RawImage).BindViewEvent(OnClickRawImage, ViewEvent.Click, this);
        GetRawImage((int)RawImages.RawImage).BindViewEvent(OnDoubleClickRawImage, ViewEvent.DoubleClick, this);

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
                Managers.App.BingRouteMode = BingRouteMode.Driving;
                break;
            case Buttons.Button_02:
                GetFindLocation(BingRouteMode.Walking);
                Managers.App.BingRouteMode = BingRouteMode.Walking;
                break;
            case Buttons.Button_03:
                GetFindLocation(BingRouteMode.Transit);
                Managers.App.BingRouteMode = BingRouteMode.Transit;
                break;
            case Buttons.Button_04:
                GetFindLocation(BingRouteMode.Bicycling);
                Managers.App.BingRouteMode = BingRouteMode.Bicycling;
                break;
            case Buttons.NavModeIcon:
                Managers.App.SetNavMode();
                break;
            case Buttons.CurrentPosIcon:
                LatLon latLon = Managers.App.MapLocationService.GetLatLon();
                if (latLon != default)
                {
                    Managers.App.MapRenderer.Center = latLon;
                }
                break;
            case Buttons.PlusIcon:
                Managers.App.MapRenderer.ZoomLevel = MapController.maxZoom;
                break;
            case Buttons.MinusIcon:
                Managers.App.MapRenderer.ZoomLevel = MapController.minZoom;
                break;
            case Buttons.CancelButton:
                Managers.UI.ClosePopupUI();
                break;
            case Buttons.Button:
                string str = inputs[0].text;
                inputs[0].text = inputs[1].text;
                inputs[1].text = str;
                obj.SetActive(false);
                break;
            case Buttons.NavStart:
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
                break;
        }

        searchButton.gameObject.SetActive(false);
        Managers.Sound.Play(SoundID.ButtonClick);
    }

    private void OnDragRawImage(PointerEventData eventData)
    {
        Vector2 dragDelta = -eventData.delta * Managers.App.polatedValue;

        Managers.App.MapController.Pan(dragDelta, false);
    }

    private void OnClickRawImage(PointerEventData eventData)
    {
        Ray ray = eventData.GetRay();

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject gameObject = hit.collider.GetComponentInChildren<Button>().gameObject;

            if (gameObject != null)
            {
                ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerClickHandler);
            }
        }
    }

    private void OnDoubleClickRawImage(PointerEventData eventData)
    {
        if (Managers.App.MapRenderer.Raycast(eventData.GetRay(), out MapRendererRaycastHit hitInfo))
        {
            LatLon latLon = new LatLon(hitInfo.Location.LatitudeInDegrees, hitInfo.Location.LongitudeInDegrees);
            ObservableList<MapPin> mapPins = Managers.App.PopupPinLayer.MapPins;

            if (mapPins.Count > 0)
            {
                mapPins.Clear();
            }
            else
            {
                MapPin mapPin = Managers.Resource.Instantiate("MapPin").GetComponent<MapPin>();
                mapPin.Location = latLon;
                mapPin.IsLayerSynchronized = false;
                mapPins.Add(mapPin);
            }
        }
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
                            GetText((int)Texts.Text_01).text = $"{Utilities.ConvertToSecondsToTime(routeLeg.travelDuration)}";
                            GetText((int)Texts.Text_02).text = $"{Utilities.ConvertToDistanceString(routeLeg.travelDistance)}";
                            GetText((int)Texts.Text_03).text = $"{Utilities.ConvertToKorean(routeLeg.travelMode)}";
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
            mapPin.IsLayerSynchronized = false;
            mapPins.Add(mapPin);
        }

        Managers.App.MapLineRenderer.gameObject.SetActive(true);
        Managers.App.MapRenderer.Center = Managers.App.startLatLon;
    }
}
