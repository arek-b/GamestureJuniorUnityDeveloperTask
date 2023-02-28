using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using UI;
using UnityEngine.Assertions;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private AudioPlayer audioPlayer = null;
        [SerializeField] private AudioClip[] equipAudioClips = null;
        [SerializeField] private Alert equipAlert = null;
        [SerializeField] private Transform inventorySlotContainer = null;
        [SerializeField, HideInInspector] private List<Slot> slots = new();

        private readonly System.Random random = new();

        private int lastAudioClipIndex = -1;

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

        private void Awake()
        {
            Assert.IsNotNull(audioPlayer);
            Assert.IsNotNull(inventorySlotContainer);
            Assert.IsNotNull(equipAlert);
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

            PlayEquipAudio();
            ShowEquipAlert(newItem.itemName);

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

        private void PlayEquipAudio()
        {
            int clipIndex = 0;
            if (equipAudioClips.Length == 0)
                return;
            else if (equipAudioClips.Length > 1)
            {
                do
                    clipIndex = random.Next(0, equipAudioClips.Length - 1);
                while (lastAudioClipIndex == clipIndex); // prevent playing the same clip twice in a row
            }

            audioPlayer.PlayClip(equipAudioClips[clipIndex]);
            lastAudioClipIndex = clipIndex;
        }

        private void ShowEquipAlert(string itemName)
        {
            equipAlert.DisplayAlert($"You have equipped {itemName}");
        }
    }
}