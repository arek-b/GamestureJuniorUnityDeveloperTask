using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Inventory
{
    public class EquippedSlotVisualizer : MonoBehaviour
    {
        [SerializeField] private float targetScaleMultiplier = 1.5f;
        [SerializeField] private float animationSpeed = 2f;
        private Vector2 defaultScale = Vector2.one;
        private Vector2 targetScale;

        private Image image;

        private Coroutine visualizeCoroutine = null;

        private void Awake()
        {
            targetScale = defaultScale * targetScaleMultiplier;
            TryGetComponent(out image);
            Assert.IsNotNull(image);
            image.raycastTarget = false;
            image.color = Color.clear;
        }

        public void Visualize()
        {
            if (visualizeCoroutine != null)
                StopCoroutine(visualizeCoroutine);
            visualizeCoroutine = StartCoroutine(VisualizeCoroutine());
        }

        private IEnumerator VisualizeCoroutine()
        {
            image.color = Color.white;
            transform.localScale = defaultScale;
            while (!ApproximatelyEqual(transform.localScale, targetScale))
            {
                transform.localScale = Vector2.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
                image.color = new Color
                {
                    r = image.color.r,
                    g = image.color.g,
                    b = image.color.b,
                    a = Mathf.Lerp(image.color.a, 0, Time.deltaTime),
                };
                yield return null;
            }
            image.color = Color.clear;
            transform.localScale = defaultScale;
        }

        private bool ApproximatelyEqual(Vector2 a, Vector2 b)
        {
            const float precision = 0.01f;
            return Vector2.Distance(a, b) < precision;
        }
    }
}