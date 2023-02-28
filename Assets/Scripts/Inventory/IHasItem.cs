namespace Inventory
{
    public interface IHasItem
    {
        ItemScriptableObject GetItem();
        void SetItem(ItemScriptableObject item);
    }
}