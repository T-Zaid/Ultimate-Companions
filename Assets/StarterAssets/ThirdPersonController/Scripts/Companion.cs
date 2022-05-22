using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : AbstractBehaviour<CompanionState>
{
    // Start is called before the first frame update
    void Start()
    {
        ChangeState(CompanionState.Idle);   
    }
}
