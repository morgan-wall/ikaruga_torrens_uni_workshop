public class CombatEffectTriggerOnDestroyed : CombatEffectTrigger
{
    protected override void Register()
    {
        base.Register();
        Source.OnDestroyed += OnDestroyed;
    }

    protected override void Deregister()
    {
        base.Deregister();
        Source.OnDestroyed -= OnDestroyed;
    }

    private void OnDestroyed(Targetable a_target, Targetable a_source)
    {
        CombatEffect.Apply(new ApplyContext { Source = Source, Target = Source }, this);
    }
}
