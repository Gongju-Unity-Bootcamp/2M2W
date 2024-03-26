using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using UnityEngine;

public class ResourceManager
{
    public Dictionary<string, GameObject> Prefabs { get; set; }
    public Dictionary<string, Texture> Textures { get; set; }
    public Dictionary<string, RenderTexture> RenderTextures { get; set; }
    public Dictionary<string, RawImage> RawImages { get; private set; }
    public Dictionary<string, Image> Images { get; private set; }
    public Dictionary<string, Sprite> Sprites { get; set; }
    public Dictionary<string, AudioClip> AudioClips { get; set; }

    public void Init()
    {
        Prefabs = new Dictionary<string, GameObject>();
        Textures = new Dictionary<string, Texture>();
        RenderTextures = new Dictionary<string, RenderTexture>();
        RawImages = new Dictionary<string, RawImage>();
        Images = new Dictionary<string, Image>();
        Sprites = new Dictionary<string, Sprite>();
        AudioClips = new Dictionary<string, AudioClip>();
    }

    private T Load<T>(Dictionary<string, T> dictionary, string path) where T : Object
    {
        if (false == dictionary.ContainsKey(path))
        {
            T resource = Resources.Load<T>(path);
            dictionary.Add(path, resource);

            return dictionary[path];
        }

        return dictionary[path];
    }

    public GameObject LoadPrefab(string path)
        => Load(Prefabs, string.Concat(Path.PREFAB, path));

    public Texture LoadTexture(string path)
        => Load(Textures, string.Concat(Path.TEXTURE, path));

    public Texture LoadRenderTexture(string path)
    => Load(RenderTextures, string.Concat(Path.RENDERTEXTURE, path));

    public RawImage LoadRawImage(string path)
        => Load(RawImages, string.Concat(Path.RAWIMAGE, path));

    public Image LoadImage(string path)
        => Load(Images, string.Concat(Path.IMAGE, path));

    public Sprite LoadSprite(string path)
        => Load(Sprites, string.Concat(Path.SPRITE, path));

    public AudioClip LoadAudioClip(string path) 
        => Load(AudioClips, string.Concat(Path.SOUND, path));

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = LoadPrefab(path);
        Debug.Assert(prefab != null);

        return Instantiate(prefab, parent);
    }

    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        GameObject gameObject = Object.Instantiate(prefab, parent);
        gameObject.name = prefab.name;

        return gameObject;
    }

    public void Destroy(GameObject gameObject)
    {
        if (gameObject != null)
        {
            Object.Destroy(gameObject);
        }
    }
}
