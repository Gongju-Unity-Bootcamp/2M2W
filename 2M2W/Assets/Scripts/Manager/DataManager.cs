using CsvHelper;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System;
using UnityEngine;

public class DataManager
{
    public Dictionary<PopupID, PopupData> Popup { get; set; }
    public Dictionary<SoundID, SoundData> Sound { get; set; }
    public Dictionary<MarkerID, MarkerData> Marker { get; set; }

    public void Init()
    {
#if UNITY_EDITOR
        Popup = ParseToDictionary<PopupID, PopupData>(string.Concat(Path.TABLE, Csv.POPUP), data => data.Id);
        Sound = ParseToDictionary<SoundID, SoundData>(string.Concat(Path.TABLE, Csv.SOUND), data => data.Id);
        Marker = ParseToDictionary<MarkerID, MarkerData>(string.Concat(Path.TABLE, Csv.MARKER), data => data.Id);
#elif UNITY_ANDROID
        string path;
        path = Resources.Load<TextAsset>(Csv.POPUP_FIX).text;
        Popup = ParseToDictionary<PopupID, PopupData>(path, data => data.Id);
        path = Resources.Load<TextAsset>(Csv.SOUND_FIX).text;
        Sound = ParseToDictionary<SoundID, SoundData>(path, data => data.Id);
        path = Resources.Load<TextAsset>(Csv.MARKER_FIX).text;
        Marker = ParseToDictionary<MarkerID, MarkerData>(path, data => data.Id);
#endif
    }

    private Dictionary<Key, Item> ParseToDictionary<Key, Item>([NotNull] string path, Func<Item, Key> keySelector)
    {
#if UNITY_EDITOR
        using StreamReader reader = new StreamReader(path);
#elif UNITY_ANDROID
        using StringReader reader = new StringReader(path);
#endif
        using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<Item>().ToDictionary(keySelector);
    }
}
