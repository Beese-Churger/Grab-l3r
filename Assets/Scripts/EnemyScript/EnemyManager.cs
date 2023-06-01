using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    int GetState();
    Vector2 GetPosition();
}
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager enemyManager;
    private List<GameObject> EnemyList;

    void Awake()
    {
        if (enemyManager == null)
        {
            enemyManager = this;
            EnemyList = new List<GameObject>();
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {

    }
    public void AddEnemies()
    {
       GameObject.FindGameObjectWithTag("Enemy");
    }
}
