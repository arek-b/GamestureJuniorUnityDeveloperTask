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
        private Item item;
        private ScrollRect scrollRect;

        private enum ItemState { Empty, NotMoving, Moving }
        private ItemState itemState = ItemState.Empty;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            scrollRect = GetComponentInParent<ScrollRect>();
            item = GetComponentInChildren<Item>();
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
            item.GetComponent<Image>().color = Color.red;
        }

        public void StartDrag(TouchState data)
        {
            itemState = ItemIsEmpty() ? ItemState.Empty : ItemState.NotMoving;
        }

        public void ContinueDrag(TouchState data)
        {
            if (itemState == ItemState.Empty)
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

            item = null;
            itemState = ItemState.Empty;
        }
    }
}
