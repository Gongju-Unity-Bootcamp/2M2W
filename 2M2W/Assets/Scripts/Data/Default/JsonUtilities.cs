using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IO;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Primitives;
using System;

public static class JsonUtilities
{
    public static string ObjectToJson(object o)
        => JsonConvert.SerializeObject(o);


    public static T JsonToObject<T>(string json)
        => JsonConvert.DeserializeObject<T>(json);


    public static void CreateJsonFile(string path, string name, string json)
    {
        FileStream file = new FileStream(string.Format("{0}/{1}.json", path, name), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(json);
        file.Write(data, 0, data.Length);
        file.Close();
    }

    public static T LoadJsonFile<T>(string path, string name)
    {
        FileStream file = new FileStream(string.Format("{0}/{1}.json", path, name), FileMode.Open);
        byte[] data = new byte[file.Length];
        file.Read(data, 0, data.Length);
        file.Close();
        string json = Encoding.UTF8.GetString(data);

        return JsonToObject<T>(json);
    }
}
