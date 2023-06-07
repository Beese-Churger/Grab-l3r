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
            EnemyGOArray = GameObject.FindGameObjectsWithTag("Enemy");
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
            enemy.FSMUpdate();
        }
    }
    public void AddEnemies()
    {
       
       foreach (GameObject enemyObject in EnemyGOArray)
       {
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

}
