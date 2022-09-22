using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.QuestSystem;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using DevionGames.InventorySystem;

public class QuestController : MonoBehaviour
{
    private float timeCount = 0f;
    public static QuestController instance;
    private GameObject pondDoorTrig;
    
    void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        pondDoorTrig = GameObject.FindGameObjectWithTag("PondDoorTrigger");
    }

    public void EnablePondDoors()
    {
        pondDoorTrig.GetComponent<Trigger>().enabled = true;
    }

    public void IncrementWelcomeQuest()
    {
        Quest quest = QuestManager.current.GetQuest("Welcome To Rivendale"); 
        if(quest != null && quest.Status == Status.Active) { 
            QuestTask task = quest.GetTask("Destroy Skeletons"); 
            task.AddProgress(1f);
            if (task.Progress == 2)
            {
                StartCoroutine(SpawnChest());
            }
        }
    }

    public void IncrementRepeatQuest()
    {
        StartCoroutine(RepeatQuest());
    } 

    IEnumerator RepeatQuest() 
    { 
        Quest quest = QuestManager.current.GetQuest("Kill Skeletons"); 
        if(quest != null && quest.Status == Status.Active) { 
            QuestTask task = quest.GetTask("Destroy Skeletons"); 
            task.AddProgress(1f);
        }
        yield return null;
    }

    IEnumerator SpawnChest()
    {
        yield return new WaitUntil(IsWelcomeQuestComplete);
        Transform testObject = GameObject.FindGameObjectWithTag("Quest1CompleteChest").GetComponentInChildren<Transform>();
        testObject.GetChild(0).gameObject.SetActive(true);
        //testObject[0].GetChild(0)
    }

    bool IsWelcomeQuestComplete()
    {
        Quest quest = QuestManager.current.GetAnyQuest("Welcome To Rivendale"); 
        return quest != null && quest.Status == Status.Completed;
    }

    public void SearchAndRescueTask1Check()
    {
        StartCoroutine(SearchAndRescueTask1Coroutine());
    }

    public void SearchAndRescueTask2Check()
    {
        StartCoroutine(SearchAndRescueTask2Coroutine());
    }

    public void SearchAndRescueTask3Check()
    {
        StartCoroutine(SearchAndRescueTask3Coroutine());
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Mage_Room_Demo")
        {
            StopAllCoroutines();
            GameObject.FindGameObjectWithTag("StartDoorTrigger").GetComponent<Trigger>().enabled = true;
            GameObject.FindGameObjectWithTag("CloseDoorTrigger").GetComponent<Trigger>().enabled = true;
        }
    }

    public void FadeOut()
    {
        GameObject.FindGameObjectWithTag("Fade").GetComponent<Animator>().Play("Fade Out");
    }

    public void FadeIn()
    {
        GameObject.FindGameObjectWithTag("Fade").GetComponent<Animator>().Play("Fade In");
    } 

    public void teleportPorthos()  
    { 
        StartCoroutine(movePorthos());
    } 
    IEnumerator movePorthos()
    {  
        //GameObject dungeonPorthos = GameObject.FindGameObjectWithTag("Porthos"); 
        //dungeonPorthos.SetActive(false);
        Transform porthosTrans = GameObject.FindGameObjectWithTag("PorthosTrans").GetComponent<Transform>(); 
        Transform Porthos = GameObject.FindGameObjectWithTag("Porthos").GetComponent<Transform>(); 
        Porthos.position = porthosTrans.position;
        Porthos.rotation = porthosTrans.rotation;
        yield return null;
    }

    IEnumerator SearchAndRescueTask1Coroutine()
    {
        //while (!(quest != null && quest.Status == Status.Completed))
        while(!(QuestManager.current.HasQuest("Note 1", Status.Completed)))
        {
            yield return new WaitForSeconds(.2f);
        }
        Quest quest = QuestManager.current.GetQuest("Search and Rescue");
        QuestTask task = quest.GetTask("Collect 1st Piece of Evidence");
        task.AddProgress(1f);
    }

    IEnumerator SearchAndRescueTask2Coroutine()
    {
        //while (!(quest != null && quest.Status == Status.Completed))
        while(!(QuestManager.current.HasQuest("Note 2", Status.Completed)))
        {
            yield return new WaitForSeconds(.2f);
        }
        Quest quest = QuestManager.current.GetQuest("Search and Rescue");
        QuestTask task = quest.GetTask("Collect 2nd Piece of Evidence");
        task.AddProgress(1f);
    }

    IEnumerator SearchAndRescueTask3Coroutine()
    {
        //while (!(quest != null && quest.Status == Status.Completed))
        while(!(QuestManager.current.HasQuest("Porthos Saved", Status.Completed)))
        {
            yield return new WaitForSeconds(.2f);
        }
        Quest quest = QuestManager.current.GetQuest("Search and Rescue");
        QuestTask task = quest.GetTask("Porthos Fate");
        task.AddProgress(1f);
    }



    public void OutputQuestData()
    {
        Debug.Log("QuestManager.current.AllQuests data:");
        List<Quest> quests = QuestManager.current.AllQuests;
        foreach (Quest quest in quests)
        {
            Debug.Log(quest.name + " status: " + quest.Status);
        }
    }

    public void OpenBossRoom()
    {
        StartCoroutine(OpenBossRoomCoroutine());
    }
    IEnumerator OpenBossRoomCoroutine()
    {
        // Check if skeletons are dead before opening door
        yield return new WaitWhile(areSkeletonsAlive);

        // open doors smoothly and play sound
        Transform door1 = GameObject.FindGameObjectWithTag("door1").GetComponent<Transform>();
        Transform door2 = GameObject.FindGameObjectWithTag("door2").GetComponent<Transform>();
        Transform door1From = GameObject.FindGameObjectWithTag("door1OldTrans").GetComponent<Transform>();
        Transform door2From = GameObject.FindGameObjectWithTag("door2OldTrans").GetComponent<Transform>();
        Transform door1To = GameObject.FindGameObjectWithTag("door1NewTrans").GetComponent<Transform>();
        Transform door2To = GameObject.FindGameObjectWithTag("door2NewTrans").GetComponent<Transform>();
        door1From.GetComponent<AudioSource>().Play();
        while (door1.rotation != door1To.rotation)
        {
            door1.rotation = Quaternion.Slerp(door1From.rotation, door1To.rotation, timeCount);
            door2.rotation = Quaternion.Slerp(door2From.rotation, door2To.rotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
            yield return null;
        }
        Debug.Log("Opened Door");
        timeCount = 0f;
    }

    public void CloseDoors()
    {
        StartCoroutine(CloseDoorsCoroutine());
    }

    IEnumerator CloseDoorsCoroutine()
    {
        Debug.Log("Closing Door");
        // close doors smoothly and play sound in reverse
        Transform door1 = GameObject.FindGameObjectWithTag("door1").GetComponent<Transform>();
        Transform door2 = GameObject.FindGameObjectWithTag("door2").GetComponent<Transform>();
        Transform door1From = GameObject.FindGameObjectWithTag("door1OldTrans").GetComponent<Transform>();
        Transform door2From = GameObject.FindGameObjectWithTag("door2OldTrans").GetComponent<Transform>();
        Transform door1To = GameObject.FindGameObjectWithTag("door1NewTrans").GetComponent<Transform>();
        Transform door2To = GameObject.FindGameObjectWithTag("door2NewTrans").GetComponent<Transform>();
        // play door open sound backwards
        //FindObjectOfType<AudioManager>().PlayReverseAudio(door1From.GetComponent<AudioSource>());
        door1From.GetComponent<AudioSource>().Play();
        while (door1.rotation != door1From.rotation)
        {
            door1.rotation = Quaternion.Slerp(door1To.rotation, door1From.rotation, timeCount);
            door2.rotation = Quaternion.Slerp(door2To.rotation, door2From.rotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
            yield return null;
        }
        timeCount = 0f;
    }

    public void OpenExitDoor()
    {
        StartCoroutine(OpenExitDoorCoroutine());
    }

    IEnumerator OpenExitDoorCoroutine()
    {
        // open doors smoothly and play sound
        Transform door3 = GameObject.FindGameObjectWithTag("door3").GetComponent<Transform>();
        Transform door3From = GameObject.FindGameObjectWithTag("door3OldTrans").GetComponent<Transform>();
        Transform door3To = GameObject.FindGameObjectWithTag("door3NewTrans").GetComponent<Transform>();
        door3.GetComponent<AudioSource>().Play();
        while (door3.rotation != door3To.rotation)
        {
            door3.rotation = Quaternion.Slerp(door3From.rotation, door3To.rotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
            yield return null;
        }
        Debug.Log("Opened Door");
        timeCount = 0f;
    } 

    public void OpenVillage()
    {
        StartCoroutine(OpenVillageCoroutine());
    }
    IEnumerator OpenVillageCoroutine()
    {

        // open doors smoothly and play sound
        Transform gate1 = GameObject.FindGameObjectWithTag("gate1").GetComponent<Transform>();
        Transform gate2 = GameObject.FindGameObjectWithTag("gate2").GetComponent<Transform>();
        Transform gate1From = GameObject.FindGameObjectWithTag("gate1OldTrans").GetComponent<Transform>();
        Transform gate2From = GameObject.FindGameObjectWithTag("gate2OldTrans").GetComponent<Transform>();
        Transform gate1To = GameObject.FindGameObjectWithTag("gate1NewTrans").GetComponent<Transform>();
        Transform gate2To = GameObject.FindGameObjectWithTag("gate2NewTrans").GetComponent<Transform>();
        //gate1From.GetComponent<AudioSource>().Play();
        while (gate1.rotation != gate1To.rotation)
        {
            gate1.rotation = Quaternion.Slerp(gate1From.rotation, gate1To.rotation, timeCount);
            gate2.rotation = Quaternion.Slerp(gate2From.rotation, gate2To.rotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
            yield return null;
        }
        Debug.Log("Opened Door");
        timeCount = 0f;
    }

    public void CloseGate()
    {
        StartCoroutine(CloseGateCoroutine());
    }

    IEnumerator CloseGateCoroutine()
    {
        Debug.Log("Closing Door");
        // close doors smoothly and play sound in reverse
        Transform gate1 = GameObject.FindGameObjectWithTag("gate1").GetComponent<Transform>();
        Transform gate2 = GameObject.FindGameObjectWithTag("gate2").GetComponent<Transform>();
        Transform gate1From = GameObject.FindGameObjectWithTag("gate1OldTrans").GetComponent<Transform>();
        Transform gate2From = GameObject.FindGameObjectWithTag("gate2OldTrans").GetComponent<Transform>();
        Transform gate1To = GameObject.FindGameObjectWithTag("gate1NewTrans").GetComponent<Transform>();
        Transform gate2To = GameObject.FindGameObjectWithTag("gate2NewTrans").GetComponent<Transform>();
        // play door open sound backwards
        //FindObjectOfType<AudioManager>().PlayReverseAudio(door1From.GetComponent<AudioSource>());
        //gate1From.GetComponent<AudioSource>().Play();
        while (gate1.rotation != gate1From.rotation)
        {
            gate1.rotation = Quaternion.Slerp(gate1To.rotation, gate1From.rotation, timeCount);
            gate2.rotation = Quaternion.Slerp(gate2To.rotation, gate2From.rotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
            yield return null;
        }
        timeCount = 0f;
    }

    public void OpenJail() 
    { 
        StartCoroutine(OpenJailCoroutine());
    } 

    IEnumerator OpenJailCoroutine() 
    { 
        Transform jailDoor = GameObject.FindGameObjectWithTag("Jail").GetComponent<Transform>();
        Transform jailDoorOpen = GameObject.FindGameObjectWithTag("JailNewTrans").GetComponent<Transform>();
        Transform jailDoorClose = GameObject.FindGameObjectWithTag("JailOldTrans").GetComponent<Transform>();
        while (jailDoor.rotation != jailDoorOpen.rotation)
        {
            Debug.Log("Hi");
            jailDoor.rotation = Quaternion.Slerp(jailDoorClose.rotation, jailDoorOpen.rotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
            yield return null;
        }
        Debug.Log("Opened Jail");
        timeCount = 0f;
    }

    public void CloseJail() 
    { 
        StartCoroutine(CloseJailCoroutine());
    } 

    IEnumerator CloseJailCoroutine() 
    { 
        Transform jailDoor = GameObject.FindGameObjectWithTag("Jail").GetComponent<Transform>();
        Transform jailDoorOpen = GameObject.FindGameObjectWithTag("JailNewTrans").GetComponent<Transform>();
        Transform jailDoorClose = GameObject.FindGameObjectWithTag("JailOldTrans").GetComponent<Transform>();
        while (jailDoor.rotation != jailDoorClose.rotation)
        {
            jailDoor.rotation = Quaternion.Slerp(jailDoorOpen.rotation, jailDoorClose.rotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
            yield return null;
        }
        Debug.Log("Opened Jail");
        timeCount = 0f;
    }

    bool areSkeletonsAlive()
    {
        return GameObject.FindGameObjectWithTag("doorSkele1").GetComponent<SkeletonController>().enabled || GameObject.FindGameObjectWithTag("doorSkele2").GetComponent<SkeletonController>().enabled;
    }

    void Update()
    {
        
    }
}
