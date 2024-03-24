using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class MapMarkerPinController : MonoBehaviour
{
    public MarkerData data;
    public Canvas[] canvas;

    public void SetMarkerPin(MarkerData markerData)
    {
        data = markerData;
        canvas[markerData.Group - 1].gameObject.SetActive(true);
        canvas[markerData.Group - 1].GetComponentInChildren<Button>().BindViewEvent(OnClickButton, ViewEvent.Click, this);
    }

    //public void OnClickButton(PointerEventData eventData)
    //    => Managers.App.MarkerData = data;

    public void OnClickButton(PointerEventData eventData)
    {
        Managers.App.MarkerData = data;
        Debug.Log("asd");
    }
}
