using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DevionGames.StatSystem;

public class XpSlide : MonoBehaviour {
	private Image XpBar; 
	Stats Player;
	StatsHandler playerStatsHandler;

	// Use this for initialization
	void Start () {
		XpBar = GetComponent<Image>();
		Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>();
		playerStatsHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsHandler>();
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(playerStatsHandler.GetStat("Exp").Value + "/" + Player.MaxXp);
		//Debug.Log(playerStatsHandler.GetStatCurrentValue("Exp"));
		XpBar.fillAmount = playerStatsHandler.GetStat("Exp").Value / Player.MaxXp;
	}
}
