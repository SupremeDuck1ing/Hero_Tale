using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoxRadius : MonoBehaviour
{
    public float radius = 10f; 

    void OnDrawGizmosSelected () { 
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, radius); 
    }
}
