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

        private bool empty = true;
        private bool isMoving = false;

        private Coroutine updateCoroutine = null;
        private Vector2 targetPosition;

        private void Awake()
        {
            targetPosition = transform.position;
            canvas = GetComponentInParent<Canvas>();
            scrollRect = GetComponentInParent<ScrollRect>();
            item = GetComponentInChildren<Item>();
            empty = item == null;
        }

        public void Tap()
        {
            item.GetComponent<Image>().color = Color.red;
        }

        public void StartDrag(TouchState data)
        {
            empty = item == null;
        }

        public void ContinueDrag(TouchState data)
        {
            if (empty)
                return;

            float dragTime = Time.realtimeSinceStartup - (float)data.startTime;
            if (dragTime < dragThresholdSeconds)
                return;

            Vector2 movement = data.position - data.startPosition;
            if (!isMoving && Mathf.Abs(movement.x) < dragHorizontalThreshold)
                return;

            if (isMoving)
                ContinueMoving(data);
            else
                StartMoving();
        }

        private void StartMoving()
        {
            isMoving = true;
            item.transform.SetParent(canvas.transform, worldPositionStays: true);
            scrollRect.vertical = false; // temporarily prevent ScrollRect from scrolling
            if (updateCoroutine == null)
            {
                updateCoroutine = StartCoroutine(UpdateCoroutine());
            }
        }

        private IEnumerator UpdateCoroutine()
        {
            while (isMoving)
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
            if (empty || !isMoving)
                return;

            scrollRect.vertical = true;
            isMoving = false;
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }
            item = null;
        }
    }
}
