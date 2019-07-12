public interface IPooledObject
{
    void OnClaimed();
    void OnRelinquished();
    void Relinquish();
}
