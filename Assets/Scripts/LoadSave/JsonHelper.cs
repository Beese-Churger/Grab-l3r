using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static List<string> GetJsonArray(string jsonArray)
    {
        jsonArray = "{ \"array\": " + jsonArray + "}";
        JsonWrapper wrapper = JsonUtility.FromJson<JsonWrapper>(jsonArray);
        return wrapper.array;
    }

    [System.Serializable]
    private class JsonWrapper
    {
        public List<string> array;
    }
}
