using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace UIInput
{
    public class TouchManager : MonoBehaviour
    {
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private string tapActionName = "Tap";
        [SerializeField] private string touchActionName = "Touch";

        private PlayerInput playerInput;

        private InputAction tapAction;
        private InputAction touchAction;

        private TouchState currentTouchState;

        private Raycaster raycaster;

        private IDraggable<TouchState> draggedObject = null;

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

            touchAction.performed += HandleTouch;
            tapAction.performed += HandleTap;
            
            Assert.IsNotNull(canvas);
            GraphicRaycaster graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            raycaster = new Raycaster(graphicRaycaster);
        }

        private void HandleTouch(InputAction.CallbackContext callbackContext)
        {
            currentTouchState = callbackContext.ReadValue<TouchState>();
            HandleDrag();
        }

        private void HandleTap(InputAction.CallbackContext _)
        {
            if (currentTouchState.phase != TouchPhase.Canceled &&
                currentTouchState.phase != TouchPhase.Ended &&
                currentTouchState.phase != TouchPhase.None)
            {
                if (raycaster.Raycast(out ITappable tappable, currentTouchState.position))
                {
                    tappable.Tap();
                }
            }
        }

        private void HandleDrag()
        {
            switch (currentTouchState.phase)
            {
                case TouchPhase.Began:
                    if (raycaster.Raycast(out IDraggable<TouchState> draggable, currentTouchState.position))
                    {
                        draggedObject = draggable;
                        draggedObject.StartDrag(currentTouchState);
                    }
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    draggedObject?.ContinueDrag(currentTouchState);
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                case TouchPhase.None:
                default:
                    draggedObject?.EndDrag(currentTouchState);
                    draggedObject = null;
                    break;
            }
        }
    }
}
