using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UIManager
{
    public void Init()
    {
        
    }

    private List<T> ParseToList<T>([NotNull] string path)
    {
        using StreamReader reader = new StreamReader(path);
        using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<T>().ToList();
    }

    /// <summary>
    /// 딕셔너리로 읽어들인 .csv 데이터 파일을 파싱하여 반환하는 메소드입니다.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Item"></typeparam>
    /// <param name="path">데이터 파일 경로</param>
    /// <param name="keySelector">키 선택</param>
    private Dictionary<Key, Item> ParseToDictionary<Key, Item>([NotNull] string path, Func<Item, Key> keySelector)
    {
        using StreamReader reader = new StreamReader(path);
        using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<Item>().ToDictionary(keySelector);
    }
}
