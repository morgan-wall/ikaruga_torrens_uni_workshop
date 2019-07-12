public class CombatEffectTriggerOnDamaged : CombatEffectTrigger
{
    protected override void Register()
    {
        base.Register();
        Source.OnDamaged += OnDamaged;
    }

    protected override void Deregister()
    {
        base.Deregister();
        Source.OnDamaged -= OnDamaged;
    }

    private void OnDamaged(float a_amount, Targetable a_target, Targetable a_source)
    {
        CombatEffect.Apply(new ApplyContext { Source = Source, Target = Source }, this);
    }
}
