using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class Alert : MonoBehaviour
    {
        [SerializeField] private float displayTimeSeconds = 2.0f;
        [SerializeField] private float animationSpeed = 10.0f;
        [SerializeField, HideInInspector] private TextMeshProUGUI tmPro = null;

        private RectTransform rectTransform = null;

        private Vector2 visibleAnchoredPosition;
        private Vector2 hiddenAnchoredPosition;

        private Coroutine displayAlertCoroutine = null;

        private void OnValidate()
        {
            if (tmPro != null)
                return;
            foreach (Transform child in transform)
                if (child.TryGetComponent(out tmPro))
                    break;
        }

        private void Awake()
        {
            if (TryGetComponent(out rectTransform))
            {
                visibleAnchoredPosition = rectTransform.anchoredPosition;
                hiddenAnchoredPosition = new Vector2
                {
                    x = visibleAnchoredPosition.x,
                    y = rectTransform.rect.height,
                };
            }
            else // there is always supposed to be a RectTransform on the Alert because it's meant to be a UI element
                throw new System.Exception($"{nameof(RectTransform)} not found on {nameof(Alert)}");

            if (TryGetComponent(out Outline outline))
                hiddenAnchoredPosition.y += outline.effectDistance.y;

            rectTransform.anchoredPosition = hiddenAnchoredPosition;
        }

        public void DisplayAlert(string alertText)
        {
            tmPro.text = alertText;
            if (displayAlertCoroutine != null)
                StopCoroutine(displayAlertCoroutine);
            displayAlertCoroutine = StartCoroutine(DisplayAlertCoroutine());
        }

        private IEnumerator DisplayAlertCoroutine()
        {
            while (!ApproximatelyEqual(rectTransform.anchoredPosition, visibleAnchoredPosition))
            {
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, visibleAnchoredPosition, Time.deltaTime * animationSpeed);
                yield return null;
            }
            rectTransform.anchoredPosition = visibleAnchoredPosition;

            yield return new WaitForSeconds(displayTimeSeconds);

            while (!ApproximatelyEqual(rectTransform.anchoredPosition, hiddenAnchoredPosition))
            {
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, hiddenAnchoredPosition, Time.deltaTime * animationSpeed);
                yield return null;
            }
            rectTransform.anchoredPosition = hiddenAnchoredPosition;
        }

        private bool ApproximatelyEqual(Vector2 a, Vector2 b)
        {
            const float precision = 0.1f;
            return Vector2.Distance(a, b) < precision;
        }
    }
}