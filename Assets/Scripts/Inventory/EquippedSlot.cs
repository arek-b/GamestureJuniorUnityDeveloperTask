using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class EquippedSlot : MonoBehaviour, IHasItem
    {
        [SerializeField] private ItemType itemType;
        private Image childImage;
        private ItemScriptableObject item;

        public ItemType ItemType => itemType;

        public delegate void EquipItem(Slot slot, EquippedSlot equippedSlot);
        public static event EquipItem OnEquipItem;

        private void Awake()
        {
            foreach (Transform child in transform)
                if (child.TryGetComponent(out childImage))
                    break;
        }

        public void Equip(Slot slotWithItem)
        {
            if (!slotWithItem.Item.NotEmpty)
                return;
            OnEquipItem?.Invoke(slotWithItem, this);
        }

        public ItemScriptableObject GetItem() => item;

        public void SetItem(ItemScriptableObject item)
        {
            if (item == null)
                return;
            this.item = item;
            childImage.sprite = item.itemSprite;
            childImage.color = Color.white;
        }
    }
}