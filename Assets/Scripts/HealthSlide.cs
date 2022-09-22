using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DevionGames.StatSystem;

public class HealthSlide : MonoBehaviour {
	private Image HealthBar;  
	StatsHandler playerStatsHandler;

	// Use this for initialization
	void Start () {
		HealthBar = GetComponent<Image>(); 
		//Player = FindObjectOfType<Stats>();
		playerStatsHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsHandler>();
	}
	
	// Update is called once per frame
	void Update () {
		//HealthBar.fillAmount = Player.CurrHealth / Player.MaxHealth;
		HealthBar.fillAmount = playerStatsHandler.GetStatCurrentValue("Health") / playerStatsHandler.GetStatValue("Health");
	}
}
