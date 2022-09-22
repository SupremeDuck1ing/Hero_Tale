using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    Transform openPos;
    Transform closePos;
    Transform[] spears = new Transform[3];
    
    public float toggleDelay = 2f;
    public float duration = 0.35f;
    [HideInInspector]
    public bool trapClosed = false;
    public AudioClip[] trapSounds;
    AudioSource audioSource;
    private float timer;
    private GameObject player;
    private bool isTrapEnabled = false;
    
    
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        openPos = gameObject.transform.GetChild(0);
        closePos = gameObject.transform.GetChild(1);

        for (int i = 0; i < spears.Length; i++)
        {
            spears[i] = gameObject.transform.GetChild(i+2);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > toggleDelay)
        {
            if (trapClosed)
            {
                trapClosed = false;
                OpenTrap();
                timer = 0;
            }
            else
            {
                if (isTrapEnabled)
                {
                    trapClosed = true;
                    CloseTrap();
                    timer = 0;
                }
                
            }
            
        }
    }

    public void EnableTrap(bool toggle)
    {
        isTrapEnabled = toggle;
    }

    public void OpenTrap()
    {
        //play some audio

        // lerp each spear
        for (int i = 0; i < spears.Length; i++)
        {
            StartCoroutine(LerpSpear(spears[i], closePos, openPos));
        }
        StartCoroutine(PlaySound());
    }

    public void CloseTrap()
    {
        //play some audio

        // lerp each spear
        for (int i = 0; i < spears.Length; i++)
        {
            StartCoroutine(LerpSpear(spears[i], openPos, closePos));
        }
        StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(duration * 0.25f);
        // Check if player is on the same floor as trap
        if (Mathf.Abs(player.transform.position.y - transform.position.y) <= 5)
        {
            PlayTrapSound();
        }
    }

    IEnumerator LerpSpear(Transform spear, Transform start, Transform stop)
    {
        float time = 0;
        
        while (time < duration)
        {
            spear.localPosition = Vector3.Lerp(new Vector3(spear.localPosition.x, spear.localPosition.y, start.localPosition.z), new Vector3(spear.localPosition.x, spear.localPosition.y, stop.localPosition.z), time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        spear.localPosition = new Vector3(spear.localPosition.x, spear.localPosition.y, stop.localPosition.z);
    }

    protected void PlayTrapSound()
    {
        AudioClip clip = RandomTrapClip();
        audioSource.PlayOneShot(clip);
    }
    protected AudioClip RandomTrapClip()
    {
        return trapSounds[UnityEngine.Random.Range(0,trapSounds.Length)];
    }
}
