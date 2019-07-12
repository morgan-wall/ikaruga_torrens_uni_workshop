using UnityEngine;

public abstract class BoundToScreen : MonoBehaviour
{
    private Vector3 m_bounds;

    protected abstract float ObjectHalfWidth { get; }
    protected abstract float ObjectHalfHeight { get; }

    protected virtual void Start()
    {
        m_bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0.0f));
    }

    private void LateUpdate()
    {
        var boundedPosition = transform.position;
        boundedPosition.x = Mathf.Clamp(boundedPosition.x, -m_bounds.x + ObjectHalfWidth, m_bounds.x - ObjectHalfWidth);
        boundedPosition.y = Mathf.Clamp(boundedPosition.y, -m_bounds.y + ObjectHalfHeight, m_bounds.y - ObjectHalfHeight);
        transform.position = boundedPosition;
    }
}
