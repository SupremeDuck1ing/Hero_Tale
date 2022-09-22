using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DevionGames.StatSystem; 
using DevionGames.QuestSystem;

public class OrcController : EnemyController
{
    protected override void Awake()
    {
        base.Awake();
        walkSpeed = 0.45f;
        runSpeed = 0.77f;
    }
}
