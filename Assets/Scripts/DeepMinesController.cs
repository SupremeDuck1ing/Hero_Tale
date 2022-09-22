using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepMinesController : MonoBehaviour
{

    public void OpenBossRoomExit()
    {
        GameObject.FindGameObjectWithTag("QuestController").GetComponent<QuestController>().OpenExitDoor();
    }

    public void OpenJailCell()
    {
        GameObject.FindGameObjectWithTag("QuestController").GetComponent<QuestController>().OpenJail();
    } 

    public void PlayBossTheme()
    {
        GameObject.FindObjectOfType<AudioManager>().PlayBossTheme();
    }

    public void StopBossTheme()
    {
        GameObject.FindObjectOfType<AudioManager>().StopBossTheme();
    }
}
