using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIInput;
using UnityEngine.InputSystem.LowLevel;

namespace Inventory
{
    public class Slot : MonoBehaviour, IDraggable<TouchState>, ITappable
    {
        [SerializeField] private float dragThresholdSeconds = 0.3f;
        [SerializeField] private float dragHorizontalThreshold = 150f;
        [SerializeField] private float dragFollowSpeed = 20f;

        private Canvas canvas;
        private GraphicRaycaster graphicRaycaster;
        private ItemInSlot item;
        private ScrollRect scrollRect;

        private Raycaster raycaster;

        private enum ItemState { Empty, NotMoving, Moving, Returning }
        private ItemState itemState = ItemState.Empty;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            scrollRect = GetComponentInParent<ScrollRect>();
            item = GetComponentInChildren<ItemInSlot>();
            raycaster = new Raycaster(graphicRaycaster);
        }

        private void Start()
        {
            if (!ItemIsEmpty())
                itemState = ItemState.NotMoving;
        }

        private bool ItemIsEmpty()
        {
            if (item == null)
                return true;
            return !item.HasItem;
        }

        public void Tap()
        {
            if (itemState != ItemState.NotMoving)
                return;
            item.GetComponent<Image>().color = Color.red; // todo: change this!
        }

        public void StartDrag(TouchState data)
        {
            itemState = ItemIsEmpty() ? ItemState.Empty : ItemState.NotMoving;
        }

        public void ContinueDrag(TouchState data)
        {
            if (itemState == ItemState.Empty || itemState == ItemState.Returning)
                return;

            float dragTime = Time.realtimeSinceStartup - (float)data.startTime;
            if (dragTime < dragThresholdSeconds)
                return;

            Vector2 movement = data.position - data.startPosition;
            if (itemState != ItemState.Moving && Mathf.Abs(movement.x) < dragHorizontalThreshold)
                return;

            if (itemState == ItemState.Moving)
                item.ContinueMoving(data.position);
            else
            {
                item.StartMoving(newParent: canvas.transform, speed: dragFollowSpeed);
                itemState = ItemState.Moving;
                scrollRect.vertical = false; // temporarily prevent ScrollRect from scrolling
            }
        }

        public void EndDrag(TouchState data)
        {
            if (itemState != ItemState.Moving)
                return;

            item.StopMoving();
            scrollRect.vertical = true;

            if (raycaster.Raycast(out EquippedSlot equippedSlot, data.position) && item.Item.itemType == equippedSlot.ItemType)
            {
                equippedSlot.Equip(item);
                itemState = ItemState.Empty;
                item = null;
            }
            else
            {
                itemState = ItemState.Returning;
                item.Return(onReturned: () => itemState = ItemState.NotMoving);
            }
        }
    }
}
