using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallEnemy : MonoBehaviour , IEnemy
{
    enum FSM
    {
        NEUTRAL,
        AGGRESSIVE,
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
            case FSM.AGGRESSIVE:
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
