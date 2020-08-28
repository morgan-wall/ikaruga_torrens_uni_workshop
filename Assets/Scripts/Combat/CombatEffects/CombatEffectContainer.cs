using UnityEngine;
using UnityEngine.Assertions;

public class CombatEffectContainer : MonoBehaviour, ICombatEffectOwner
{
    private static CombatFilter[] k_nilFilters = new CombatFilter[] { };

    [SerializeField]
    private bool m_applyOnStart = default;

    [SerializeField]
    private Transform m_filters = default;

    [SerializeField]
    private EffectSet m_effectSetPrefab = default;

    private EffectSet m_effectSet;
    private Targetable m_targetable;
    private CombatFilter[] m_combatFilters;
    private CombatEffect[] m_combatEffects;

    private void Awake()
    {
        if (m_effectSetPrefab != null)
        {
            m_effectSet = GameObject.Instantiate(m_effectSetPrefab, transform);
        }

        m_targetable = GetComponentInParent<Targetable>();
        Assert.IsNotNull(m_targetable);
    }

    private void Start()
    {
        if (m_applyOnStart)
        {
            CombatEffect.Apply(new ApplyContext { Target = m_targetable, Source = m_targetable }, this);
        }
    }


    #region ICombatEffectOwner

    public ICombatEffectOwner Owner { get { return null; } }
    public Targetable Source { get { return m_targetable; } }
    public EffectSet EffectSet { get { return m_effectSet; } }

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
