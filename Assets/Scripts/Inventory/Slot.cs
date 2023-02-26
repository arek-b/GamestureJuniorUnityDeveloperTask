using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
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

        private Coroutine updateCoroutine = null;
        private Vector2 targetPosition;

        private enum ItemState { Empty, NotMoving, Moving }
        private ItemState itemState = ItemState.Empty;

        private void Awake()
        {
            targetPosition = transform.position;
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
                ContinueMoving(data);
            else
                StartMoving();
        }

        private void StartMoving()
        {
            itemState = ItemState.Moving;
            item.transform.SetParent(canvas.transform, worldPositionStays: true);
            scrollRect.vertical = false; // temporarily prevent ScrollRect from scrolling
            if (updateCoroutine == null)
            {
                updateCoroutine = StartCoroutine(UpdateCoroutine());
            }
        }

        private IEnumerator UpdateCoroutine()
        {
            while (itemState == ItemState.Moving)
            {
                item.transform.position = Vector2.Lerp(item.transform.position, targetPosition, Time.deltaTime * dragFollowSpeed);
                yield return null;
            }
        }

        private void ContinueMoving(TouchState data)
        {
            targetPosition = data.position;
        }

        public void EndDrag(TouchState data)
        {
            if (itemState != ItemState.Moving)
                return;

            scrollRect.vertical = true;
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }
            item = null;
            itemState = ItemState.Empty;
        }
    }
}
