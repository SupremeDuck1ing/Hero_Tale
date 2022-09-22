using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.QuestSystem;

public class QuestInteract : MonoBehaviour
{
    


    public void talkTo()
    {
        Quest quest = QuestManager.current.GetQuest("Skull-Cracker");  
        Quest welcomeQuest = QuestManager.current.GetQuest("Welcome Quest");  
        if(quest != null && quest.Status == Status.Completed) { 
            QuestTask task = welcomeQuest.GetTask("Elder"); 
            task.AddProgress(1f);
        }
    }
}
