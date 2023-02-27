using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class ItemInSlot : MonoBehaviour, IHasItem
    {
        [SerializeField] private ItemScriptableObject item = null;

        private Image image;
        private Transform originalParent;
        private Vector2 originalLocalPosition;

        public bool NotEmpty => item != null;
        public ItemScriptableObject GetItem() => item;

        private Coroutine updateCoroutine = null;
        private float speed = 1f;
        private Vector2 targetPosition;

        private bool isReturning = false;

        private void OnValidate()
        {
            if (!NotEmpty)
                return;
            if (TryGetComponent(out Image image))
                image.sprite = item.itemSprite;
        }

        private void Awake()
        {
            if (TryGetComponent(out image) && !NotEmpty)
                image.color = Color.clear;

            originalParent = transform.parent;
            originalLocalPosition = transform.localPosition;
            
            targetPosition = transform.position;
        }

        public void StartMoving(Transform newParent, float speed)
        {
            if (isReturning)
                return;

            this.speed = speed;
            image.raycastTarget = false;
            transform.SetParent(newParent, worldPositionStays: true);
            if (updateCoroutine == null)
            {
                updateCoroutine = StartCoroutine(UpdateCoroutine());
            }
        }

        private IEnumerator UpdateCoroutine()
        {
            while (true) // coroutine stopped manually via StopCoroutine()
            {
                transform.position = Vector2.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
                yield return null;
            }
        }
        public void ContinueMoving(Vector2 position)
        {
            if (isReturning)
                return;

            targetPosition = position;
        }

        public void StopMoving()
        {
            if (isReturning)
                return;

            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }
        }

        public void Return(Action onReturned)
        {
            isReturning = true;
            StartCoroutine(ReturnCoroutine(onReturned));
        }

        private IEnumerator ReturnCoroutine(Action onReturned)
        {
            if (item != null)
            {
                Vector2 startingPosition = transform.position;
                for (float lerpValue01 = 0f; lerpValue01 < 1f; lerpValue01 += Time.deltaTime * speed)
                {
                    transform.position = Vector2.Lerp(startingPosition, originalParent.position, lerpValue01);
                    yield return null;
                }
            }
            ResetParentPositionAndRaycastTarget();
            isReturning = false;
            onReturned();
        }

        private void ResetParentPositionAndRaycastTarget()
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalLocalPosition;
            image.raycastTarget = true;
        }

        public void SetItem(ItemScriptableObject item)
        {
            this.item = item;
            if (item == null)
            {
                image.sprite = null;
                image.color = Color.clear;
            }
            else
            {
                image.sprite = item.itemSprite;
                image.color = Color.white;
            }
        }
    }
}