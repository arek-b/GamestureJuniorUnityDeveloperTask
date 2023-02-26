using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class Item : MonoBehaviour
    {
        private Image image;

        public bool HasItem { get; private set; } = false;

        private void Awake()
        {
            if (TryGetComponent(out image))
            {
                HasItem = image.sprite != null;
                if (!HasItem)
                    image.color = Color.clear;
            }
        }
    }
}