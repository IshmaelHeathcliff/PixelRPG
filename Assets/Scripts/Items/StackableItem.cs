using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newStackableItem", menuName = "SO/Items/New Stackable Item")]
    public class StackableItem : Item
    {
        [SerializeField] int maxCountPerStack;
        
        public new static StackableItem GetFromID(string itemID)
        { 
            var item = Item.GetFromID(itemID);
            if (item is StackableItem stackableItem)
            {
                return stackableItem;
            }
            else
            {
                return null;
            }
        }

        public int GetMaxCountPerStack()
        {
            return maxCountPerStack;
        }

    }
}