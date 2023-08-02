using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance = null;
    private int currentPlayerID = 0;

    public Dictionary<int, PlayerData> playerDataDictionary = new ();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    public void SavePlayerData(string playerName, float playerTime)
    {
        // Create a new PlayerData object with the current player information
        PlayerData playerData = new (playerName, playerTime); // Replace with actual player data
        while (playerDataDictionary.ContainsKey(currentPlayerID))
            currentPlayerID++;

        // Add or update the player data in the dictionary
        playerDataDictionary[currentPlayerID] = playerData;
        // Serialize the PlayerData object to JSON
        string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented);

        // Save the JSON data to a file using the player ID as the filename
        string filePath = Path.Combine(Application.persistentDataPath, "playerData_" + currentPlayerID + ".json");
        File.WriteAllText(filePath, jsonData);

        Debug.Log("Player data has been saved successfully.");
    }

    public void LoadPlayerData(string chosenProfile, float time)
    {
        // Load player data for the current player ID
        string filePath = Path.Combine(Application.persistentDataPath, "playerData_" + currentPlayerID + ".json");

        if (File.Exists(filePath))
        {
            // Read the JSON data from the file
            string jsonData = File.ReadAllText(filePath);

            // Deserialize the JSON data to a PlayerData object
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(jsonData);

            // Add or update the player data in the dictionary
            playerDataDictionary[currentPlayerID] = playerData;

            Debug.Log("Player data has been loaded successfully.");
            Debug.Log("Player Name: " + playerData.playerName);
            Debug.Log("Time Taken: " + playerData.timeTaken);
        }
        else
        {
            Debug.LogWarning("Player data file not found. Creating a new one.");
            SavePlayerData(chosenProfile, time); // Create a new player data file with default values
        }
    }
    
    public bool LoadManagerData(int index)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "playerData_" + index + ".json");

        if (File.Exists(filePath))
        {
            // Read the JSON data from the file
            string jsonData = File.ReadAllText(filePath);

            // Deserialize the JSON data to a PlayerData object
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(jsonData);

            // Add or update the player data in the dictionary
            playerDataDictionary[index] = playerData;
            return true;
        }

        return false;
    }
    public bool CheckForExistingName(string name)
    {
        bool playerNameExists = playerDataDictionary.Any(playerData => playerData.Value.playerName == name);

        if (playerNameExists)
            return true;
        return false;
    }
    // Method to change the current player ID (e.g., when switching between players)
    public void SetCurrentPlayerID(int newPlayerID)
    {
        currentPlayerID = newPlayerID;
    }
    private void OnEnable()
    {
        int counter = 0;
        while (LoadManagerData(counter))
        {
            counter++;
        }
    }
}