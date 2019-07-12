public class CombatEffectDisableShield : CombatEffect
{
    protected override bool Apply(ApplyContext a_applyContext)
    {
        var shipTargetable = a_applyContext.Target as ShipTargetable;
        if (shipTargetable == null)
        {
            return false;
        }

        var ship = Ship.Get(shipTargetable);
        if (ship == null)
        {
            return false;
        }

        ship.Context.DeactivateShield();
        return true;
    }
}
