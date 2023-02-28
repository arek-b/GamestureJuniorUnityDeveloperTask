using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIInput
{
    public class Raycaster
    {
        private GraphicRaycaster graphicRaycaster = null;
        private PointerEventData pointerEventData;
        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        public Raycaster(GraphicRaycaster graphicRaycaster)
        {
            this.graphicRaycaster = graphicRaycaster;
            pointerEventData = new PointerEventData(EventSystem.current);
        }

        public bool Raycast<T>(out T result, Vector2 position) where T : class
        {
            raycastResults.Clear();
            pointerEventData.position = position;
            graphicRaycaster.Raycast(pointerEventData, raycastResults);

            foreach (RaycastResult item in raycastResults)
            {
                if (item.gameObject.TryGetComponent(out result))
                {
                    return true;
                }
            }
            result = null;
            return false;
        }
    }
}