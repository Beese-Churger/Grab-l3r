using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager enemyManager;
    [SerializeField]private List<EnemyBaseClass> EnemyList;
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
            enemy.FSMUpdate();
        }
    }
    public void AddEnemies()
    {
       GameObject[] EnemyGOArray = GameObject.FindGameObjectsWithTag("Enemy");
       foreach (GameObject enemyObject in EnemyGOArray)
       {
           EnemyList.Add(enemyObject.GetComponent<EnemyBaseClass>());
       }
    }

}
