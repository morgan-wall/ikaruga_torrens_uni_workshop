using UnityEngine;

public class PooledObjectKillzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D a_collider2D)
    {
        var pooledObject = a_collider2D.GetComponentInParent<IPooledObject>();
        if (pooledObject == null)
        {
            Debug.LogWarning($"Non pooled object object ({a_collider2D.name}) hit killzone ({name}).");
            return;
        }

        pooledObject.Relinquish();
    }
}
