using UnityEngine;
using System.Collections.Generic;

public class RandomNameGenerator : MonoBehaviour
{
    public static RandomNameGenerator GenInstance = null;
    public List<string> prefixes = new () { "Al", "Br", "Dra", "El", "Kor", "Zan" };
    public List<string> middleParts = new (){ "a", "en", "in", "or", "u", "y" };
    public List<string> suffixes = new () { "dor", "gan", "is", "lin", "thal", "vyn" };

    public string GetRandomName()
    {
        string name = GetRandomElement(prefixes) + GetRandomElement(middleParts) + GetRandomElement(suffixes) + "_" + RandomNumberGenerator();
        return name;
    }

    private T GetRandomElement<T>(List<T> list)
    {
        if (list.Count > 0)
        {
            int randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
        else
        {
            Debug.LogWarning("The list is empty. Cannot retrieve random element.");
            return default(T);
        }
    }
    private string RandomNumberGenerator()
    {
        string number = "";
        for (int i = 0; i < 4; ++i)
        {
            int randomIndex = Random.Range(0, 10);
            number += randomIndex;
        }
        return number;
    }
    private void Awake()
    {
        if (GenInstance == null)
            GenInstance = this;
    }
    //private void Start()
    //{
    //    string randomName = GetRandomName();
    //    Debug.Log("Random Name: " + randomName);
    //}
}
