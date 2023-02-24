using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using Inventory;

namespace Input
{
    public class TouchManager : MonoBehaviour
    {
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private string tapActionName = "Tap";
        [SerializeField] private string touchActionName = "Touch";

        private PlayerInput playerInput;

        private InputAction tapAction;
        private InputAction touchAction;

        private GraphicRaycaster graphicRaycaster = null;

        private PointerEventData pointerEventData;
        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        private void Awake()
        {
            if (Touchscreen.current == null)
            {
                TouchSimulation.Enable();
            }

            playerInput = GetComponent<PlayerInput>();
            playerInput.SwitchCurrentControlScheme(Touchscreen.current);

            tapAction = playerInput.actions.FindAction(tapActionName);
            touchAction = playerInput.actions.FindAction(touchActionName);

            tapAction.performed += TapAction_performed;
            touchAction.performed += TouchAction_performed;
            
            Assert.IsNotNull(canvas);
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();

            pointerEventData = new PointerEventData(EventSystem.current);
        }

        private void TapAction_performed(InputAction.CallbackContext obj)
        {
            if (Raycast(out Slot slot))
            {
                slot.Tapped();
            }
        }

        private void TouchAction_performed(InputAction.CallbackContext obj)
        {
            TouchState touchState = obj.ReadValue<TouchState>();
            pointerEventData.position = touchState.position;

        }

        private bool Raycast<T>(out T result) where T : MonoBehaviour
        {
            raycastResults.Clear();
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
