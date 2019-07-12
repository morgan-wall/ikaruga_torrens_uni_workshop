using UnityEngine;

public class CombatEffectDamage : CombatEffect
{
    [SerializeField]
    private float m_amount;

    protected override bool Apply(ApplyContext a_applyContext)
    {
        Targetable.Damage(m_amount, a_applyContext.Target, a_applyContext.Source);
        return true;
    }
}
