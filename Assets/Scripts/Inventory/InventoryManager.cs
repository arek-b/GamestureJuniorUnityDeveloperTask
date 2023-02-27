using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private Transform inventorySlotContainer = null;
        [SerializeField, HideInInspector] private List<Slot> slots = new();

        private void OnValidate()
        {
            if (inventorySlotContainer == null)
            {
                slots.Clear();
                return;
            }

            RebuildSlotList();
        }

        private void RebuildSlotList()
        {
            slots.Clear();
            foreach (Transform child in inventorySlotContainer)
                if (child.TryGetComponent(out Slot slot))
                    slots.Add(slot);
        }

        private void OnEnable()
        {
            EquippedSlot.OnEquipItem += EquippedSlot_OnEquipItem;
        }

        private void OnDisable()
        {
            EquippedSlot.OnEquipItem -= EquippedSlot_OnEquipItem;
        }

        private void EquippedSlot_OnEquipItem(Slot slot, EquippedSlot equippedSlot)
        {
            if (slot == null || slot.Item == null || equippedSlot == null)
                return;
            ItemScriptableObject oldItem = equippedSlot.GetItem();
            ItemScriptableObject newItem = slot.Item.GetItem();
            if (newItem == null)
                return;

            equippedSlot.SetItem(newItem);
            slot.Item.SetItem(oldItem);

            if (oldItem == null)
                RemoveGap();
        }

        private void RemoveGap()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].Item.GetItem() == null)
                {
                    slots[i].transform.SetAsLastSibling();
                    RebuildSlotList();
                    break;
                }
            }
        }
    }
}