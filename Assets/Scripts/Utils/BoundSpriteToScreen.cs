//#define BOUNDS_INCLUDE_SPRITE_WIDTH
#define BOUNDS_INCLUDE_SPRITE_HEIGHT

using UnityEngine;
using UnityEngine.Assertions;

public class BoundSpriteToScreen : BoundToScreen
{
    private float m_objectHalfWidth;
    private float m_objectHalfHeight;

    protected override float ObjectHalfWidth { get { return m_objectHalfWidth; } }
    protected override float ObjectHalfHeight { get { return m_objectHalfHeight; } }

    protected override void Start()
    {
        base.Start();

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        Assert.IsNotNull(spriteRenderer);

        m_objectHalfWidth = 0;
        m_objectHalfHeight = 0;
#if BOUNDS_INCLUDE_SPRITE_WIDTH
        m_objectHalfWidth = spriteRenderer.bounds.size.x / 2.0f;
#endif // BOUNDS_INCLUDE_SPRITE_WIDTH
#if BOUNDS_INCLUDE_SPRITE_HEIGHT
        m_objectHalfHeight = spriteRenderer.bounds.size.y / 2.0f;
#endif // BOUNDS_INCLUDE_SPRITE_HEIGHT
    }
}
