using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

namespace Inventory
{
    public class Slot : MonoBehaviour
    {
        public void Tapped()
        {
            Debug.Log("Slot tapped");
            GetComponent<Image>().color = Color.red;
        }

        public void Dragged()
        {

        }
    }
}
