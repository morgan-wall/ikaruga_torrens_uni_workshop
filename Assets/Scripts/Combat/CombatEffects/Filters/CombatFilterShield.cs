using UnityEngine;

public class CombatFilterShield : CombatFilter
{
    [SerializeField]
    private bool m_isActive;

    public override bool Check(ApplyContext a_applyContext)
    {
        var shipTargetable = a_applyContext.Target as ShipTargetable;
        if (shipTargetable == null)
        {
            return !m_isActive;
        }

        var ship = Ship.Get(shipTargetable);
        if (ship == null)
        {
            return !m_isActive;
        }

        return ship.Context.ShieldActive == m_isActive;
    }
}
