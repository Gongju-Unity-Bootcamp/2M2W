using CsvHelper;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System;

public class DataManager
{
    public Dictionary<StoryboardID, StoryboardData> Storyboard { get; set; }
    public Dictionary<SoundID, SoundData> Sound { get; set; }

    public void Init()
    {
#if UNITY_EDITOR
        Storyboard = ParseToDictionary<StoryboardID, StoryboardData>(string.Concat(Path.TABLE, Csv.STORYBOARD), data => data.Id);
        Sound = ParseToDictionary<SoundID, SoundData>(string.Concat(Path.TABLE, Csv.SOUND), data => data.Id);
//#else
//        TextAsset Storyboard = Resources.Load<TextAsset>(Csv.STORYBOARD_FIX);
//        Storyboard = ParseToDictionary<StoryboardID, StoryboardData>(Storyboard.text, data => data.Id);
//        TextAsset Sound = Resources.Load<TextAsset>(Csv.SOUND_FIX);
//        Sound = ParseToDictionary<SoundID, SoundData>(Sound.text, data => data.Id);
#endif
    }

    private Dictionary<Key, Item> ParseToDictionary<Key, Item>([NotNull] string path, Func<Item, Key> keySelector)
    {
#if UNITY_EDITOR
        using StreamReader reader = new StreamReader(path);
#else
        using StringReader reader = new StringReader(path);
#endif
        using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<Item>().ToDictionary(keySelector);
    }
}
