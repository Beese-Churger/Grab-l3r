using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseClass : MonoBehaviour
{
    public virtual void FSMUpdate()
    {

    }
    public virtual int GetWeight()
    {
        return 0;
    }
    public virtual void SetStatus(bool b_Status)
    {

    }
}
