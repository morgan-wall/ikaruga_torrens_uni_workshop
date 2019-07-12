using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Ship))]
public class ShipTargetable : Targetable
{
    private Ship m_ship;

    public override Element DefensiveElement
    {
        get
        {
            if (m_ship.Context != null)
            {
                return m_ship.Context.ShieldElement;
            }
            return Element.None;
        }
    }

    public override Element OffensiveElement
    {
        get
        {
            return Element.None;
        }
    }

    protected override float Health
    {
        get
        {
            return m_ship.Context.Health;
        }
        set
        {
            m_ship.Context.Health = Mathf.Max(value, 0.0f);
        }
    }

    protected override bool Damage(float a_amount, Targetable a_source)
    {
        var previousHealth = Health;
        Health -= a_amount;
        return previousHealth != Health;
    }

    protected override void Destroy(Targetable a_source)
    {
        base.Destroy(a_source);
        ObjectPoolManager.Instance.Relinquish(m_ship.Context.Definition.Prefab.gameObject, m_ship.gameObject);
    }

    private void Awake()
    {
        m_ship = GetComponent<Ship>();
        Assert.IsNotNull(m_ship);
    }
}
