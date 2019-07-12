using UnityEngine.Assertions;

public class CombatEffectTriggerOnShieldActivated : CombatEffectTrigger
{
    private Ship m_ship;

    protected override void Register()
    {
        base.Register();
        RegisterInternal();
    }

    protected override void Deregister()
    {
        base.Deregister();
        DeregisterInternal();
    }

    private void RegisterInternal()
    {
        if (m_ship != null
            && m_ship.Context != null)
        {
            m_ship.Context.OnShipShieldActivated += OnShipShieldActivated;
        }
    }

    private void DeregisterInternal()
    {
        if (m_ship != null
            && m_ship.Context != null)
        {
            m_ship.Context.OnShipShieldActivated -= OnShipShieldActivated;
        }
    }

    private void OnShipShieldActivated(ShipContext a_context)
    {
        CombatEffect.Apply(new ApplyContext { Source = Source, Target = Source }, this);
    }

    private void OnShipContextBound(Ship a_ship)
    {
        Assert.IsTrue(m_ship == a_ship);
        if (Registered)
        {
            RegisterInternal();
        }
    }

    private void Start()
    {
        m_ship = Source.GetComponent<Ship>();
        if (m_ship == null)
        {
            return;
        }

        m_ship.OnShipContextBound += OnShipContextBound;

        if (m_ship.Context != null)
        {
            OnShipContextBound(m_ship);
        }
    }
}
