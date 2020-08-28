using UnityEngine;

public class ProjectileEffects : MonoBehaviour, ICombatEffectOwner
{
    private static CombatFilter[] k_nilFilters = new CombatFilter[] { };

    [SerializeField]
    private Transform m_filters = default;

    [SerializeField]
    private EffectSet m_effectSetPrefab = default;

    private EffectSet m_effectSet;
    private Targetable m_backingTargetable;
    private CombatFilter[] m_combatFilters;
    private CombatEffect[] m_combatEffects;

    public void Init(Targetable a_backingTargetable)
    {
        m_backingTargetable = a_backingTargetable;
    }

    private void Awake()
    {
        if (m_effectSetPrefab != null)
        {
            m_effectSet = GameObject.Instantiate(m_effectSetPrefab, transform);
        }
    }

    private void Start()
    {
        if (m_filters == null)
        {
            m_combatFilters = k_nilFilters;
        }
        else
        {
            m_combatFilters = m_filters.GetComponentsInDirectChildren<CombatFilter>();
        }

        m_combatEffects = transform.GetComponentsInDirectChildren<CombatEffect>();
    }


    #region ICombatEffectOwner

    public ICombatEffectOwner Owner { get { return null; } }
    public Targetable Source { get { return m_backingTargetable; } }
    public EffectSet EffectSet { get { return m_effectSet; } }
    public CombatFilter[] Filters { get { return m_combatFilters; } }
    public CombatEffect[] Effects { get { return m_combatEffects; } }

    #endregion // ICombatEffectOwner
}
