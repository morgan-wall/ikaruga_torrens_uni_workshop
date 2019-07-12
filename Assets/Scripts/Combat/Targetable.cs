using UnityEngine;

public abstract class Targetable : MonoBehaviour
{
    public delegate void DamageDelegate(float a_amount, Targetable a_target, Targetable a_source);
    public delegate void DestroyedDelegate(Targetable a_target, Targetable a_source);
    
    private bool m_markedForDestruction;
    private Targetable m_destructionSource;

    public event DamageDelegate OnDamaged;
    public event DamageDelegate OnDamagedBy;
    public event DestroyedDelegate OnDestroyed;
    public event DestroyedDelegate OnDestroyedBy;

    public abstract Element DefensiveElement { get; }
    public abstract Element OffensiveElement { get; }
    protected abstract float Health { get; set; }

    private void OnDamagedCallback(float a_amount, Targetable a_target)
    {
        if (OnDamaged != null)
        {
            OnDamaged(a_amount, a_target, this);
        }
    }

    private void OnDamagedByCallback(float a_amount, Targetable a_source)
    {
        if (OnDamagedBy != null)
        {
            OnDamagedBy(a_amount, this, a_source);
        }
    }

    private void OnDestroyedCallback(Targetable a_target)
    {
        if (OnDestroyed != null)
        {
            OnDestroyed(a_target, this);
        }
    }

    private void OnDestroyedByCallback(Targetable a_source)
    {
        if (OnDestroyedBy != null)
        {
            OnDestroyedBy(this, a_source);
        }
    }

    public static void Damage(float a_amount, Targetable a_target, Targetable a_source)
    {
        if (a_amount <= 0.0f)
        {
            return;
        }

        if (!a_target.Damage(a_amount, a_source))
        {
            return;
        }

        a_target.OnDamagedByCallback(a_amount, a_source);
        a_source.OnDamagedCallback(a_amount, a_target);

        if (a_target.Health > 0.0f)
        {
            return;
        }

        MarkForDestruction(a_target, a_source);
    }

    public static void MarkForDestruction(Targetable a_target, Targetable a_source)
    {
        a_target.MarkForDestruction(a_source);
    }

    protected void MarkForDestruction(Targetable a_source)
    {
        if (m_markedForDestruction)
        {
            return;
        }
        m_markedForDestruction = true;
        m_destructionSource = a_source;
    }

    protected abstract bool Damage(float a_amount, Targetable a_source);

    protected virtual void Destroy(Targetable a_source)
    {
        OnDestroyedByCallback(a_source);
        a_source.OnDestroyedCallback(this);
    }

    private void LateUpdate()
    {
        if (m_markedForDestruction)
        {
            Destroy(m_destructionSource);
        }

        m_markedForDestruction = false;
        m_destructionSource = null;
    }
}
