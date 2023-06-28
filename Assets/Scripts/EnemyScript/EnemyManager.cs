using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager enemyManager;
    [SerializeField]private List<EnemyBaseClass> EnemyList;
    private GameObject[] EnemyGOArray;
    void Awake()
    {
        if (enemyManager == null)
        {
            enemyManager = this;
            AddEnemies();
            
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
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
        foreach (GameObject enemyObject in EnemyGOArray)
        {
            if (!EnemyList.Contains(enemyObject.GetComponent<EnemyBaseClass>()))
                EnemyList.Add(enemyObject.GetComponent<EnemyBaseClass>());

        }
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

}
