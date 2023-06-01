using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEnemy : MonoBehaviour, IEnemy
{
    enum FSM
    {
        NEUTRAL,
        PATROL,
        NUM_STATE
    };

    private FSM current;
    [SerializeField] private float x,y;
    // Start is called before the first frame update
    void Start()
    {
        current = FSM.NEUTRAL;
        gameObject.transform.position = new Vector2(x,y);

    }
    public void FSMUpdate()
    {
        switch (current)
        {
            case FSM.NEUTRAL:
            break;
            case FSM.PATROL:
            break;
        }
    }
    public int GetState()
    {
        return (int)current;
    }
    public Vector2 GetPosition()
    {
        return new Vector2(x,y);
    }
}
