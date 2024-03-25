using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using System.IO;
using System;
using Application = UnityEngine.Device.Application;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class NavDetailsPopup : UIPopup
{
    private enum Images
    {
        MaskImage
    }

    private enum Buttons
    {
        ReportButton,
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
    private LatLon endLatLon;

    public override void Init()
    {
        base.Init();

        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetImage((int)Images.MaskImage).sprite = Managers.Resource.LoadSprite(Managers.App.MarkerData.Path);

        mapPins = Managers.App.MapPinLayer.MapPins;
        Managers.App.startLatLon = Managers.App.MapLocationService.GetLatLon();
        endLatLon = new LatLon(Managers.App.MarkerData.Latitude, Managers.App.MarkerData.Longitude);
        Managers.App.endLatLon = endLatLon;
        SetDetailTexts();

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
                            Managers.App.itineraryItems = routeLeg.itineraryItems;
                        }
                    }
                }
            }
        }, BingRouteMode.Walking));

        Managers.App.BingRouteMode = BingRouteMode.Walking;

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
        mapPins.Clear();
        Managers.App.MapPinSubLayer.MapPins.Clear();
        Managers.App.MapLineRenderer.gameObject.SetActive(false);

        switch (button)
        {
            case Buttons.ReportButton:
                Application.OpenURL("tel://112");
                break;
            case Buttons.Button:
                LatLon latLon = Managers.App.MapLocationService.GetLatLon();
                if (latLon != default)
                {
                    MapPin startPin = Managers.Resource.Instantiate("StartPin").GetComponent<MapPin>();
                    startPin.Location = latLon;
                    startPin.IsLayerSynchronized = false;
                    mapPins.Add(startPin);
                    MapPin endPin = Managers.Resource.Instantiate("EndPin").GetComponent<MapPin>();
                    Managers.App.endLatLon = endLatLon;
                    endPin.Location = Managers.App.endLatLon;
                    endPin.IsLayerSynchronized = false;
                    mapPins.Add(endPin);
                    GetRenderRoute();
                    Managers.UI.OpenPopup<ViewNavPopup>();
                }
                else
                {
                    Managers.UI.OpenPopup<ConsentPopup>();
                }
                break;
            case Buttons.Button_01:
                MarkerData data = Managers.App.MarkerData;
                Texture2D texture = Managers.Resource.LoadSprite(data.Path).texture;
                string fileName = data.Name;
                if (texture != null)
                {
                    NativeGallery.SaveImageToGallery(texture, Path.ALBUM, fileName);
                }
                break;
            case Buttons.Button_02:
                string url = Managers.App.MarkerData.Link;
#if UNITY_ANDROID
                Utilities.AndroidOpenURL(url);
#endif
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
        
        StartCoroutine(endLatLon.GetAddress(response =>
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
