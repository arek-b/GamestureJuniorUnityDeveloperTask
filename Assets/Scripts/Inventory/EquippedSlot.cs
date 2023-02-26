using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class EquippedSlot : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;
        private Image image;
        private Image childImage;

        public ItemType ItemType => itemType;

        private void Awake()
        {
            TryGetComponent(out image);
            foreach (Transform child in transform)
                if (child.TryGetComponent(out childImage))
                    break;
        }

        public void Equip(ItemInSlot item)
        {
            if (!item.HasItem)
                return;
            childImage.sprite = item.GetSprite();
            childImage.color = Color.white;
            item.Clear();
        }
    }
}