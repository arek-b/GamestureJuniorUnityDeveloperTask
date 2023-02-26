using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class Item : MonoBehaviour
    {
        private Image image;
        private Transform originalParent;
        private Vector2 originalLocalPosition;

        public bool HasItem { get; private set; } = false;

        private Coroutine updateCoroutine = null;
        private float speed;
        private Vector2 targetPosition;

        private void Awake()
        {
            if (TryGetComponent(out image))
            {
                HasItem = image.sprite != null;
                if (!HasItem)
                    image.color = Color.clear;
            }
            originalParent = transform.parent;
            originalLocalPosition = transform.localPosition;
            
            targetPosition = transform.position;
        }

        public void StartMoving(Transform newParent, float speed)
        {
            this.speed = speed;
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
            targetPosition = position;
        }

        public void StopMoving()
        {
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }
        }

        public void Clear()
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalLocalPosition;
            HasItem = false;
            image.sprite = null;
            image.color = Color.clear;
        }

        public Sprite GetSprite()
        {
            if (!HasItem)
                return null;
            return image.sprite;
        }
    }
}