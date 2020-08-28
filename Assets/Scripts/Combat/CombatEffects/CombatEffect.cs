using UnityEngine;

public struct ApplyContext
{
    public Targetable Source;
    public Targetable Target;

    public ApplyContext(Targetable a_source, Targetable a_target)
    {
        Source = a_source;
        Target = a_target;
    }
}

public struct RevertContext
{
    public Targetable Source;
    public Targetable Target;
    public int EffectMask;

    public RevertContext(Targetable a_source, Targetable a_target, int a_effectMask)
    {
        Source = a_source;
        Target = a_target;
        EffectMask = a_effectMask;
    }
}

public class CombatEffect : MonoBehaviour, ICombatEffectOwner
{
    private static CombatFilter[] k_nilFilters = new CombatFilter[] { };

    [SerializeField]
    private Transform m_filters = default;

    private Targetable m_source = null;
    private CombatFilter[] m_combatFilters = null;
    private CombatEffect[] m_combatEffects = null;
    private ICombatEffectOwner m_owner = null;

    public static int Apply(ApplyContext a_applyContext, ICombatEffectOwner a_combatEffects)
    {
        int effectMask = 0;
        for (int i = 0; i < a_combatEffects.Effects.Length; ++i)
        {
            var combatEffect = a_combatEffects.Effects[i];

            bool passesFilters = true;
            for (int j = 0; j < combatEffect.Filters.Length; ++j)
            {
                var combatFilter = combatEffect.Filters[j];
                if (!combatFilter.Check(a_applyContext))
                {
                    passesFilters = false;
                    break;
                }
            }

            if (!passesFilters)
            {
                continue;
            }

            if (combatEffect.Apply(a_applyContext))
            {
                effectMask |= (1 << i);
            }
        }
        return effectMask;
    }

    public static void Revert(RevertContext a_revertContext, ICombatEffectOwner a_combatEffects)
    {
        for (int i = 0; i < a_combatEffects.Effects.Length; ++i)
        {
            if ((a_revertContext.EffectMask & (1 << i)) == 0)
            {
                continue;
            }

            a_combatEffects.Effects[i].Revert(a_revertContext);
        }
    }

    protected virtual bool Apply(ApplyContext a_applyContext)
    {
        return false;
    }

    protected virtual void Revert(RevertContext a_revertContext)
    {
    }

    private void Awake()
    {
        m_owner = transform.parent != null ? transform.parent.GetComponentInParent<ICombatEffectOwner>() : null;
        m_source = GetComponentInParent<Targetable>();
    }


    #region ICombatEffectOwner

    public ICombatEffectOwner Owner { get { return m_owner; } }
    public Targetable Source { get { return m_source; } }
    public EffectSet EffectSet { get { return Owner != null ? Owner.EffectSet : null; } }

    public CombatFilter[] Filters
    {
        get
        {
            if (m_combatFilters == null)
            {
                if (m_filters == null)
                {
                    m_combatFilters = k_nilFilters;
                }
                else
                {
                    m_combatFilters = m_filters.GetComponentsInDirectChildren<CombatFilter>();
                }
            }
            return m_combatFilters;
        }
    }

    public CombatEffect[] Effects
    {
        get
        {
            if (m_combatEffects == null)
            {
                m_combatEffects = transform.GetComponentsInDirectChildren<CombatEffect>();
            }
            return m_combatEffects;
        }
    }

    #endregion // ICombatEffectOwner
}
