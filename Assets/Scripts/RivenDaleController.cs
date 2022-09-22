using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivenDaleController : MonoBehaviour
{
    public void IncrementWelcomeQuest()
    {
        GameObject.FindGameObjectWithTag("QuestController").GetComponent<QuestController>().IncrementWelcomeQuest();
    }
}
