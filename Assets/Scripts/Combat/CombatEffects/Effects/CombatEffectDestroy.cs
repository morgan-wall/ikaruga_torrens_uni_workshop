public class CombatEffectDestroy : CombatEffect
{
    protected override bool Apply(ApplyContext a_applyContext)
    {
        Targetable.MarkForDestruction(a_applyContext.Target, a_applyContext.Source);
        return true;
    }
}
