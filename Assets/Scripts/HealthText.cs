using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI;
using DevionGames.StatSystem;

public class HealthText : MonoBehaviour
{
    public Text Health;
	StatsHandler playerStatsHandler;

	// Use this for initialization
	void Start () {
		Health = GetComponent<Text>(); 
        //Player = FindObjectOfType<Stats>();
        playerStatsHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsHandler>();
	}

    // Update is called once per frame
    void Update()
    { 
        if (playerStatsHandler.GetStatCurrentValue("Health") >= 0) 
        { 
            Health.text = ((int)playerStatsHandler.GetStatCurrentValue("Health")).ToString() + "/" + playerStatsHandler.GetStatValue("Health").ToString();
        } 
        else 
        { 
            Health.text = "0/" + playerStatsHandler.GetStatValue("Health").ToString();
        }
    }
}
