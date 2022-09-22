using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI;
using DevionGames.InventorySystem;
using DevionGames.StatSystem;
using DevionGames.UIWidgets;

public class Stats : MonoBehaviour
{ 
    //Initial Stat Values
    public float MaxHealth = 100f;
    public float MaxXp = 100f; 
    public float CurrXp;  
    public float EnemyDamage = 10f; 
    public float DamageTaken;  
    public float DamageDealt = 50f; 
    public float Armor;
    public float XpGain = 20f;

    Collider KnightCollider;

    StatsHandler handler;
    UIWidget menuUIWidget;
    UIWidget optionsUIWidget;

    void Awake () { 
        CurrXp = 0f;  
        DamageTaken = 0f;
        KnightCollider = GetComponent<Collider>();
        menuUIWidget = GameObject.FindGameObjectWithTag("EscMenu").GetComponent<UIWidget>();
        optionsUIWidget = GameObject.FindGameObjectWithTag("OptionsMenu").GetComponent<UIWidget>();
    }

    void Start()
    {
        handler = GetComponent<StatsHandler>();
        CurrXp = handler.GetStatCurrentValue("Exp");
    }

    void Update () { 
        if (menuUIWidget.IsVisible || optionsUIWidget.IsVisible)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }


        //Test Code For taking Damage and Gaining XP
		if (Input.GetKeyDown(KeyCode.T)) { 
            //Hit();
            //GameObject.FindGameObjectWithTag("Fade").GetComponent<Animator>().Play("Fade Out");
            // if (Time.timeScale != 0.7f)
            // {
            //     Time.timeScale = 0.7f;
            // }
            // else
            // {
            //     Time.timeScale = 1;
            // }
        }

        if(Input.GetKeyDown(KeyCode.Y)) { 
            //GainXp();
            //GameObject.FindGameObjectWithTag("Fade").GetComponent<Animator>().Play("Fade In");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Devion Health:" + handler.GetStatValue("Health"));
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Devion Vitality:" + handler.GetStatValue("Vitality"));
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Devion Level:" + handler.GetStatValue("Level"));
        }

        //LevelUp();
	} 

    public void Hit () {   
        //Test Damage Code 
        DamageTaken = (EnemyDamage * (1 - (Armor / 10)));
        handler.ApplyDamage("Health", DamageTaken);
        Debug.Log(transform.name + " takes " + DamageTaken + " damage.");
        Debug.Log("Devion Health Stat:"+handler.GetStatCurrentValue("Health"));
        Debug.Log("Devion Health Stat:"+handler.GetStat("Health").Value);

        if (handler.GetStatCurrentValue("Health") <= 0) { 
            Debug.Log(transform.name + " is dead.");
        }
    } 

    public void GainXp (int xpAmount) {  
        //Test XP Code
        CurrXp += xpAmount;
        Debug.Log("Devion Exp Stat:" + handler.GetStat("Exp").Value);
        handler.GetStat("Exp").Add(xpAmount);

        Debug.Log(transform.name + " gains " + xpAmount + " Experience.");
        Debug.Log("Devion Exp Stat:" + handler.GetStat("Exp").Value);
        //Checks if Max Xp is reached, then performs leveling actions
        if (handler.GetStat("Exp").Value >= MaxXp)
        {
            LevelUp();
        }
    } 

    public void LevelUp () {  
        
        handler.GetStat("Level").Add(1f);
        Debug.Log(transform.name + " has leveled up to Level " + handler.GetStat("Level").Value);
        Debug.Log("Devion Vitality:"+handler.GetStat("Vitality").Value);
        MaxXp += XpGain;
        handler.GetStat("Exp").Subtract(MaxXp);
        CurrXp = handler.GetStat("Exp").Value;
        MaxHealth = handler.GetStat("Health").Value + handler.GetStat("Vitality").Value;

        DamageDealt += 10f;
    }

    public void AddHealth(int health)
    {
        handler.GetStat("Health").Add(health);
    }
}
