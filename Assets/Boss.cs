using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // Start is called before the first frame update
    enum ATTACK
    {
        SLAM,
        CRUSH,
        GRINDER,
        NUM_ATTACKS
    };
    enum FSM
    {
        NEUTRAL,
        MOVE,
        ATTACK,
        NUM_STATE
    };


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
