using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Projectile))]
public class ProjectileTargetable : Targetable
{
    private static readonly float k_fixedHealth = 1.0f;

    private Projectile m_projectile;

    public override Element DefensiveElement
    {
        get
        {
            if (m_projectile.Context != null)
            {
                switch (m_projectile.Context.Definition.Element)
                {
                    case Element.None:
                        break;
                    case Element.Red:
                        return Element.Blue;
                    case Element.Blue:
                        return Element.Red;

                    default:
                        throw new NotImplementedException();
                }
            }
            return Element.None;
        }
    }

    public override Element OffensiveElement
    {
        get
        {
            if (m_projectile.Context != null)
            {
                return m_projectile.Context.Definition.Element;
            }
            return Element.None;
        }
    }

    protected override float Health
    {
        get
        {
            return k_fixedHealth;
        }
        set
        {
        }
    }

    protected override bool Damage(float a_amount, Targetable a_source)
    {
        return false;
    }

    protected override void Destroy(Targetable a_source)
    {
        base.Destroy(a_source);
        ObjectPoolManager.Instance.Relinquish(m_projectile.Context.Definition.Prefab.gameObject, m_projectile.gameObject);
    }

    private void Awake()
    {
        m_projectile = GetComponent<Projectile>();
        Assert.IsNotNull(m_projectile);
    }
}
