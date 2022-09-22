using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI;
using DevionGames.StatSystem;

public class LevelText : MonoBehaviour
{
    public Text Level;
    Stats Player;
    StatsHandler playerStatsHandler;
	

	// Use this for initialization
	void Start () {
		Level = GetComponent<Text>();
        Player = FindObjectOfType<Stats>();
        playerStatsHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsHandler>();
	}

    // Update is called once per frame
    void Update()
    {
        Level.text = playerStatsHandler.GetStat("Level").Value.ToString();
    }
}
