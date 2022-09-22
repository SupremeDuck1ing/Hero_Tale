using DevionGames.InventorySystem;
using System.Linq;
using UnityEngine;

namespace DevionGames.QuestSystem.Integrations.InventorySystem
{
    [System.Serializable]
    public class ItemReward : Reward
    {
        [ItemPicker(true)]
        [SerializeField]
        protected Item m_Item;
        [SerializeField]
        protected int amount = 1;
        [SerializeField]
        protected string m_Window = "Inventory";

        /// <summary>
        /// Give the reward to player. 
        /// </summary>
        public override bool GiveReward()
        {
            Item instance = InventoryManager.CreateInstance(m_Item, amount, new ItemModifierList());
            return ItemContainer.AddItem(m_Window, instance);
        }

        /// <summary>
        /// Display the reward in QuestWindow. 
        /// </summary>
        public override void DisplayReward(RectTransform parent, int order)
        {
            ItemContainer rewardPrefab = this.m_DisplayRewardPrefab.GetComponent<ItemContainer>();

            ItemContainer[] containers = parent.GetComponentsInChildren<ItemContainer>();
            ItemContainer rewardContainer = containers.FirstOrDefault(x => x.Name == rewardPrefab.Name);
            if (rewardContainer == null ) {
                rewardContainer = GameObject.Instantiate(rewardPrefab);
                rewardContainer.transform.SetParent(parent, false);
            }
            rewardContainer.transform.SetSiblingIndex(order);
            Item instance = InventoryManager.CreateInstance(m_Item, amount, new ItemModifierList());
            rewardContainer.StackOrAdd(instance);
        }
    }
}