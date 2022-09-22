using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.QuestSystem;
using DevionGames.StatSystem;

public class SaveStatsAndQuests : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Save()
    {
        QuestManager.Save("Player");
        StatsManager.Save("Player Stats");
    }

    public void Load()
    {
        // Will hopefully load the latest save...
        QuestManager.Load("Player");
        StatsManager.Load("Player Stats");
    }
}
