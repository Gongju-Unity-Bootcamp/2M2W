using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource[] audioSources;

    public void Init()
    {
        int audioCount = Enum.GetValues(typeof(SoundType)).Length;
        string[] soundTypeNames = Enum.GetNames(typeof(SoundType));

        audioSources = new AudioSource[audioCount];

        for (int index = 0; index < audioSources.Length; ++index)
        {
            GameObject gameObject = new GameObject(soundTypeNames[index]);
            audioSources[index] = gameObject.AddComponent<AudioSource>();
            gameObject.transform.parent = transform;
        }

        audioSources[(int)SoundType.BGM].loop = true;
    }

    public void Play(SoundID id)
    {
        SoundData data = Managers.Data.Sound[id];
        SoundType type = id.ConvertToSoundType();

        AudioSource audioSource = audioSources[(int)type];
        audioSource.volume = data.Volume;
        AudioClip audioClip = GetAudioClip(data.Name);

        if (type == SoundType.VFX)
        {
            audioSource.PlayOneShot(audioClip);
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    public void Stop(SoundType type)
    {
        AudioSource audioSource = audioSources[(int)type];
        audioSource.Stop();
    }

    public void Pause(SoundType type)
    {
        AudioSource audioSource = audioSources[(int)type];
        audioSource.Pause();
    }

    public void UnPause(SoundType type)
    {
        AudioSource audioSource = audioSources[(int)type];
        audioSource.UnPause();
    }

    public float GetAudioClipLength(string path) 
        => GetAudioClip(path).length;

    private AudioClip GetAudioClip(string path) 
        => Managers.Resource.LoadAudioClip(path);
}
