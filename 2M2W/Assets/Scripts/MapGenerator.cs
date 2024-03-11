using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public RawImage mapRawImage;

    [Header("Input Map Infomation")]

    public string strBaseURL = "";
    public string latitude = "";
    public string longitude = "";
    public int level = 14;
    public int mapWidth;
    public int mapHeight;
    public string strAPIKey = "";
    public string secretKey = "";

    private void Start()
    {
        mapRawImage = GetComponent<RawImage>();
        StartCoroutine(MapLoader());
    }

    IEnumerator MapLoader()
    {
        string str = strBaseURL + "?w=" + mapWidth.ToString() + "&h=" + 
            mapHeight.ToString() + "&center=" + longitude + "," + latitude + "&level=" + level.ToString();

        Debug.Log(str.ToString());

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(str);

        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", strAPIKey);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY", secretKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            mapRawImage.texture = DownloadHandlerTexture.GetContent(request);
        }
    }
}