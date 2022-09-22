using DevionGames.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.QuestSystem.Integrations.InventorySystem
{
    [System.Serializable]
    public class GatherQuestTask : QuestTask
    {
        [HeaderLine("Gather Task")]
        [ItemPicker(true)]
        [SerializeField]
        protected Item m_Item;
        [SerializeField]
        protected string m_Window = "Inventory";
        [SerializeField]
        protected bool m_RemoveItems = true;

        //Activate the quest
        public override void Activate()
        {
            base.Activate();
            //Check the inventory for change
            UnityTools.StartCoroutine(CheckInventory());
        }

        //Checks the inventory every second
        private IEnumerator CheckInventory() {
            while (owner.Status == Status.Active)
            {
                yield return new WaitForSeconds(1);
                //if (owner.Status != Status.Active) yield break;

                //Check if the player has the required item amount.
                if (ItemContainer.HasItem(this.m_Window, m_Item, (int)this.m_RequiredProgress))
                {
                    //Set the current task progress to the required progress -> Task completed
                    SetProgress(this.m_RequiredProgress);
                }else {
                    //Player does not have enough items so set the amount he has to the current progress.
                    SetProgress(ItemContainer.GetItemAmount(this.m_Window, this.m_Item.Id));
                }
            }
        }

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
    }
}