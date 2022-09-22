using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour
{
    public Transform playerTrans;
    public Transform cameraTrans;

    public static GameController instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        playerTrans = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        cameraTrans = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        DeletePlayerPrefs();
    }

    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SaveGame()
    {
        // 1
        HeroesTaleSave save = CreateSaveGameObject();

        // 2
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("HeroesTale Game Saved");
    }

    private HeroesTaleSave CreateSaveGameObject()
    {
        HeroesTaleSave save = new HeroesTaleSave();
        // int i = 0;
        // foreach (GameObject targetGameObject in targets)
        // {
        //     Target target = targetGameObject.GetComponent<Target>();
        //     if (target.activeRobot != null)
        //     {
        //     save.livingTargetPositions.Add(target.position);
        //     save.livingTargetsTypes.Add((int)target.activeRobot.GetComponent<Robot>().type);
        //     i++;
        //     }
        // }
        save.playerPosition.Add(playerTrans.position.x);
        save.playerPosition.Add(playerTrans.position.y);
        save.playerPosition.Add(playerTrans.position.z);
       
        save.playerAngles.Add(playerTrans.eulerAngles.x);
        save.playerAngles.Add(playerTrans.eulerAngles.y);
        save.playerAngles.Add(playerTrans.eulerAngles.z);

        save.cameraPosition.Add(cameraTrans.position.x);
        save.cameraPosition.Add(cameraTrans.position.y);
        save.cameraPosition.Add(cameraTrans.position.z);

        save.cameraAngles.Add(cameraTrans.eulerAngles.x);
        save.cameraAngles.Add(cameraTrans.eulerAngles.y);
        save.cameraAngles.Add(cameraTrans.eulerAngles.z);

        return save;
    }

    public void LoadGame()
    { 
        // 1
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            HeroesTaleSave save = (HeroesTaleSave)bf.Deserialize(file);
            file.Close();

            // // 3
            // for (int i = 0; i < save.livingTargetPositions.Count; i++)
            // {
            // int position = save.livingTargetPositions[i];
            // Target target = targets[position].GetComponent<Target>();
            // target.ActivateRobot((RobotTypes)save.livingTargetsTypes[i]);
            // target.GetComponent<Target>().ResetDeathTimer();
            // }

            // // 4
            // shotsText.text = "Shots: " + save.shots;
            // hitsText.text = "Hits: " + save.hits;
            // shots = save.shots;
            // hits = save.hits;
            playerTrans.position = new Vector3(save.playerPosition[0],save.playerPosition[1],save.playerPosition[2]);
            playerTrans.transform.eulerAngles = new Vector3(save.playerAngles[0],save.playerAngles[1],save.playerAngles[2]);

            cameraTrans.transform.position = new Vector3(save.cameraPosition[0],save.cameraPosition[1],save.cameraPosition[2]);
            cameraTrans.transform.eulerAngles = new Vector3(save.cameraAngles[0],save.cameraAngles[1],save.cameraAngles[2]);
            
            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }
}
