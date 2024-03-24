using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FlowerVideo : MonoBehaviour
{
    private VideoPlayer _videoPlayer;
    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
    }

    private void PlayVideo()
    {
        StartCoroutine(OnVideoPlay());
    }

    IEnumerator OnVideoPlay()
    {
        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = "https://www.youtube.com/watch?v=qzFLl2ZoUvY";

        _videoPlayer.Play();
        yield return null;
    }

}
