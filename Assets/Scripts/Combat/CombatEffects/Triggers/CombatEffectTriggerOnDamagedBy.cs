public class CombatEffectTriggerOnDamagedBy : CombatEffectTrigger
{
    protected override void Register()
    {
        base.Register();
        Source.OnDamagedBy += OnDamagedBy;
    }

    protected override void Deregister()
    {
        base.Deregister();
        Source.OnDamagedBy -= OnDamagedBy;
    }

    private void OnDamagedBy(float a_amount, Targetable a_target, Targetable a_source)
    {
        CombatEffect.Apply(new ApplyContext { Source = Source, Target = Source }, this);
    }
}
