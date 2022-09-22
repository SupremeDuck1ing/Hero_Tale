using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.UIWidgets;
using DevionGames.StatSystem;

public class NotiifcationHandler : MonoBehaviour
{
    NotificationTrigger notificationTrigger;
    StatsHandler handler;
    float prevExp;
    // Start is called before the first frame update
    void Start()
    {
        notificationTrigger = GetComponent<NotificationTrigger>();
        handler = GetComponent<StatsHandler>();
        prevExp = handler.GetStat("Exp").Value;
    }

    public void UpdateNotificationOutput(int type)
    {
        switch (type)
        {
            case 0:
                float currExp = handler.GetStat("Exp").Value;
                bool resetXp = false;
                if (currExp == 0)
                {
                    currExp = GetComponent<Stats>().MaxXp - 20;
                    resetXp = true;
                }
                notificationTrigger.options[0].text = "Gained " + (currExp - prevExp) + " Experience";
                if (resetXp)
                {
                    prevExp = 0;
                }
                else
                {
                    prevExp = currExp;
                }
                break;
            default:
                break;
        }
    }

    public void DisplayNotification(string type)
    {
        switch (type)
        {
            case "exp":
                UpdateNotificationOutput(0);
                notificationTrigger.AddNotification(0);
                break;
            default:
                Debug.Log("Unknown notification type request");
                break;
        }
    }
}
