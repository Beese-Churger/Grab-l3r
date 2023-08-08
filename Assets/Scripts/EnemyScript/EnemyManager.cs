using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Serializable]
    public class EnemyArrayData
    {
        public List<EnemyData> enemyData;
        public EnemyArrayData(List<EnemyData> enemyData)
        {
            this.enemyData = enemyData;
        }
    }
    [Serializable]
    public class EnemyData
    {
        public int level;
        public float[] position;

        public EnemyData(int level, float[] position)
        {
            this.level = level;
            this.position = position;
        }
    }
    public static EnemyManager enemyManager;
    public List<EnemyBaseClass> EnemyList;
    private GameObject[] EnemyGOArray;
    void Awake()
    {
        if (enemyManager == null)
        {
            enemyManager = this;
            AddEnemies();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        foreach (EnemyBaseClass enemy in EnemyList)
        {
            if (enemy != null)
                enemy.FSMUpdate();
        }
    }
    public void AddEnemies()
    {
        EnemyGOArray = GameObject.FindGameObjectsWithTag("Enemy");
        EnemyList = new();
        foreach (GameObject enemyObject in EnemyGOArray)
        {
            if (!EnemyList.Contains(enemyObject.GetComponent<EnemyBaseClass>()))
                EnemyList.Add(enemyObject.GetComponent<EnemyBaseClass>());

        }
       // Debug.Log("Loading Enemies");
    }
    public int GetEnemyWeight(GameObject enemy)
    {
        foreach (GameObject enemyObject in EnemyGOArray)
        {
            if (enemy == enemyObject)
            {
                return enemyObject.GetComponent<EnemyBaseClass>().GetWeight();
            }
        }
        return -1;
    }
    public void SetEnemyWeight(GameObject enemy, int weight)
    {
        enemy.GetComponent<EnemyBaseClass>().SetWeight(weight);
    }
    public void SetEnemyStatus(GameObject enemy, bool newStatus)
    {
        foreach (GameObject enemyObject in EnemyGOArray)
        {
            if (enemy == enemyObject)
            {
                enemyObject.GetComponent<EnemyBaseClass>().SetStatus(newStatus);
                return;
            }
        }
         
    }
    public int GetEnemyType(GameObject enemy)
    {
        return enemy.GetComponent<EnemyBaseClass>().GetEnemyType();
    }
    public void ClearEnemyList()
    {
        for (int i = EnemyList.Count - 1; i >= 0; --i)
        {
            Destroy(EnemyList[i].gameObject);
        }
        EnemyList.Clear();
        //Debug.Log(EnemyList.Count);
    }
    public void SaveEnemyPosToJson()
    {
        int currentLevel = LevelManager.instance.GetCurrentLevelIndex() - 1;
        List<EnemyData> storeEnemyData = new();
        if (EnemyList.Count > 0)
        {
            foreach (EnemyBaseClass enemy in EnemyList)
            {
                float[] pos = new float[3] { enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z };
                EnemyData data = new(currentLevel, pos);
                storeEnemyData.Add(data);
            }
            EnemyArrayData serialize = new(storeEnemyData);
            string jsonData = JsonConvert.SerializeObject(serialize, Formatting.Indented);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "Level" + currentLevel + "EnemyData.json"), jsonData);
        }
        else
        {
            Debug.LogWarning("Trying to save null data");
        }
    }
    public void LoadEnemyPosFromJson()
    {
        int currentLevel = LevelManager.instance.GetCurrentLevelIndex() - 1;
        //int currentLevel = 1;

        string filePath = Path.Combine(Application.persistentDataPath, "Level" + currentLevel + "EnemyData.json");
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            EnemyArrayData data = JsonConvert.DeserializeObject<EnemyArrayData>(jsonData);
            List<EnemyData> temp = data.enemyData;
            //Debug.Log(temp.Count);
            if (temp.Count == EnemyList.Count)
            {
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    Vector3 pos = new(temp[i].position[0], temp[i].position[1], temp[i].position[2]);
                    EnemyList[i].gameObject.transform.position = pos;
                }
                //Debug.Log(EnemyList.Count);
                Debug.Log("Loaded Enemy Position");
            }
            else
            {
                Debug.LogWarning("Error while loading data");
            }
        }
        else
        {
            Debug.LogWarning("File not found: " + filePath);
            return;
        }
    }
    public void EraseEnemyData()
    {
        int currentLevel = LevelManager.instance.GetCurrentLevelIndex() - 1;
        //Debug.Log(currentLevel);
        string filePath = Path.Combine(Application.persistentDataPath, "Level" + currentLevel + "EnemyData.json");
        File.Delete(filePath);
    }

}