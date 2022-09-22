using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.StatSystem;

public class SpearController : MonoBehaviour
{
    [SerializeField]
    public float spearCD = 3.5f;
    [SerializeField]
    public AudioClip playerGruntClip;
    //private Object m_Data=null;
    private StatsHandler playerStatsHandler;
    private bool isSpearOnCD;
    // Start is called before the first frame update
    void Start()
    {
        playerStatsHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && !isSpearOnCD)
        {
            StartCoroutine(SpearCD());
            Debug.Log("Player Hit :)");
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }
            playerStatsHandler.ApplyDamage("Health", 20f);
            AnimationEvent evt = new AnimationEvent();
            evt.stringParameter = "SFX";
            evt.floatParameter = 1f;
            evt.objectReferenceParameter = playerGruntClip;
            playerStatsHandler.SendMessage("PlayAudio", evt);
        }
    }

    IEnumerator SpearCD()
    {
        isSpearOnCD = true;
        yield return new WaitForSeconds(spearCD);
        isSpearOnCD = false;
    }
}
