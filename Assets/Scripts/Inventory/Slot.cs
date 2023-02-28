using UnityEngine;
using UnityEngine.UI;
using UIInput;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Assertions;

namespace Inventory
{
    public class Slot : MonoBehaviour, IDraggable<TouchState>, ITappable
    {
        [SerializeField] private float dragThresholdSeconds = 0.3f;
        [SerializeField] private float dragHorizontalThreshold = 150f;
        [SerializeField] private float dragFollowSpeed = 20f;

        private Canvas canvas;
        private GraphicRaycaster graphicRaycaster;
        private ItemInSlot item;
        private ScrollRect scrollRect;

        private EquippedSlot equippedSlotSword = null;
        private EquippedSlot equippedSlotShield = null;

        private Raycaster raycaster;

        private enum ItemState { Empty, NotMoving, Moving, Returning }
        private ItemState itemState = ItemState.Empty;

        public ItemInSlot Item => item;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            scrollRect = GetComponentInParent<ScrollRect>();
            item = GetComponentInChildren<ItemInSlot>();
            EquippedSlot[] equippedSlots = canvas.GetComponentsInChildren<EquippedSlot>(includeInactive: false);
            foreach (EquippedSlot equippedSlot in equippedSlots)
            {
                if (equippedSlot.ItemType == ItemType.Sword)
                    equippedSlotSword = equippedSlot;
                else
                    equippedSlotShield = equippedSlot;
            }
            raycaster = new Raycaster(graphicRaycaster);
            Assert.IsNotNull(canvas);
            Assert.IsNotNull(graphicRaycaster);
            Assert.IsNotNull(scrollRect);
            Assert.IsNotNull(item);
            Assert.IsNotNull(equippedSlotSword);
            Assert.IsNotNull(equippedSlotShield);
            Assert.IsNotNull(raycaster);
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
            return !item.NotEmpty;
        }

        public void Tap()
        {
            if (itemState != ItemState.NotMoving)
                return;

            if (Item.GetItem().itemType == ItemType.Sword)
                equippedSlotSword.Equip(this);
            else
                equippedSlotShield.Equip(this);
        }

        public void StartDrag(TouchState data)
        {
            itemState = ItemIsEmpty() ? ItemState.Empty : ItemState.NotMoving;
        }

        public void ContinueDrag(TouchState data)
        {
            if (itemState == ItemState.Empty || itemState == ItemState.Returning)
                return;

            float dragTime = Time.realtimeSinceStartup - (float)data.startTime;
            if (dragTime < dragThresholdSeconds)
                return;

            Vector2 movement = data.position - data.startPosition;
            if (itemState != ItemState.Moving && Mathf.Abs(movement.x) < dragHorizontalThreshold)
                return;

            if (itemState == ItemState.Moving)
                item.ContinueMoving(data.position);
            else
            {
                item.StartMoving(newParent: canvas.transform, speed: dragFollowSpeed, position: data.position);
                itemState = ItemState.Moving;
                scrollRect.vertical = false; // temporarily prevent ScrollRect from scrolling
            }
        }

        public void EndDrag(TouchState data)
        {
            if (itemState != ItemState.Moving)
                return;

            item.StopMoving();
            scrollRect.vertical = true;

            if (raycaster.Raycast(out EquippedSlot equippedSlot, data.position) && item.GetItem().itemType == equippedSlot.ItemType)
            {
                equippedSlot.Equip(this);
            }

            itemState = ItemState.Returning;
            item.Return(onReturned: () => itemState = ItemState.NotMoving);
        }
    }
}
