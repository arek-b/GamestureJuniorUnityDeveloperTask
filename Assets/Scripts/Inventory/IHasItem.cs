using System.Collections;
using UnityEngine;

namespace Inventory
{
    public interface IHasItem
    {
        ItemScriptableObject GetItem();
        void SetItem(ItemScriptableObject item);
    }
}