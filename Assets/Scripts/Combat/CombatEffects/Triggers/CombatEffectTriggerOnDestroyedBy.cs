public class CombatEffectTriggerOnDestroyedBy : CombatEffectTrigger
{
    protected override void Register()
    {
        base.Register();
        Source.OnDestroyedBy += OnDestroyedBy;
    }

    protected override void Deregister()
    {
        base.Deregister();
        Source.OnDestroyedBy -= OnDestroyedBy;
    }

    private void OnDestroyedBy(Targetable a_target, Targetable a_source)
    {
        CombatEffect.Apply(new ApplyContext { Source = Source, Target = Source }, this);
    }
}
