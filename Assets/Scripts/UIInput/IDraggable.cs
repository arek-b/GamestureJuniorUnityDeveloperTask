namespace UIInput
{
    public interface IDraggable<T>
    {
        void StartDrag(T data);
        void ContinueDrag(T data);
        void EndDrag(T data);
    }
}