using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DevionGames.StatSystem; 
using DevionGames.QuestSystem;


public class SkeletonController : EnemyController
{
    protected override void Grunt()
    {
        AudioClip clip = GetRandomGruntClip();
        audioSource.PlayOneShot(clip, 0.3f);
    }
}
