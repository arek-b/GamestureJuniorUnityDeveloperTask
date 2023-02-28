using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Inventory/Item")]
    public class ItemScriptableObject : ScriptableObject
    {
        public Sprite itemSprite;
        public ItemType itemType;
        public string itemName;
    }
}