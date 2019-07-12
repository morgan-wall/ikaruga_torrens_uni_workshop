using UnityEngine;

public class CombatEffectEffect : CombatEffect
{
    [SerializeField]
    private string m_beginId;

    [SerializeField]
    private string m_endId;

    protected override bool Apply(ApplyContext a_applyContext)
    {
        if (EffectSet == null)
        {
            return false;
        }

        var model = Model.Get(a_applyContext.Target);
        if (model != null)
        {
            EffectSet.Begin(m_beginId, model);
        }
        return false;
    }

    protected override void Revert(RevertContext a_revertContext)
    {
        if (EffectSet == null)
        {
            return;
        }

        var model = Model.Get(a_revertContext.Target);
        if (model != null)
        {
            EffectSet.End(m_beginId, model);
        }
    }
}
