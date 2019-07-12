public interface ICombatEffectOwner
{
    ICombatEffectOwner Owner { get; }
    Targetable Source { get; }
    EffectSet EffectSet { get; }
    CombatFilter[] Filters { get; }
    CombatEffect[] Effects { get; }
}
