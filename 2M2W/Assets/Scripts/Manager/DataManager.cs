using CsvHelper;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System;

public class DataManager
{
    public void Init()
    {
#if UNITY_EDITOR

#else

#endif
    }

    private List<T> ParseToList<T>([NotNull] string path) where T : class
    {
#if UNITY_EDITOR
        using StreamReader reader = new StreamReader(path);
#else
        using StringReader reader = new StringReader(path);
#endif
        using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<T>().ToList();
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
