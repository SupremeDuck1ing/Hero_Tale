using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using DevionGames.QuestSystem;


namespace DevionGames.QuestSystem
{
    [System.Serializable]
    public class CompleteQuestTask : QuestTask
    {
        [HeaderLine("Quest Completion Task")]
        [SerializeField]
        protected string subQuest;
        //Activate the quest 
        public override void Activate()
        {
            base.Activate();
            //Check the inventory for change 
            UnityTools.StartCoroutine(CheckQuest());
        }
        
        //Checks the inventory every second
        private IEnumerator CheckQuest() { 
            Quest quest = QuestManager.current.GetQuest(subQuest);
            while (owner.Status == Status.Active)
            {
                yield return new WaitForSeconds(1);
                if(quest.Status == Status.Active) 
                { 
                    SetProgress(this.m_RequiredProgress);
                }
            }
        } 
        /*
        public override void OnQuestCompleted()
        {
            //remove the items from the inventory when the quest is completed
            if (m_RemoveItems)
                ItemContainer.RemoveItem(this.m_Window, m_Item, (int)RequiredProgress);
        }
        //This is called when a task is loaded from save. We do not activate the quest so we need to restart inventory check.
        public override void SetObjectData(Dictionary<string, object> data)
        {
            base.SetObjectData(data);
            UnityTools.StartCoroutine(CheckInventory());
        }
        */
    } 
    
}