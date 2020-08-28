using UnityEngine;

[CreateAssetMenu(menuName = "Game/Definitions/Behaviour")]
public class BehaviourDefinition : ScriptableObject, IContextDefinition<BehaviourContext>
{
    [SerializeField]
    private bool m_canUsePrimaryWeapon;

    [SerializeField]
    private bool m_canUseSecondaryWeapon;

    [SerializeField]
    private float m_timeBetweenPrimaryWeaponShots;

    [SerializeField]
    private float m_timeBetweenSecondaryWeaponShots;

    public bool CanUsePrimaryWeapon { get { return m_canUsePrimaryWeapon; } }
    public bool CanUseSecondaryWeapon { get { return m_canUseSecondaryWeapon; } }
    public float TimeBetweenPrimaryWeaponShots { get { return m_timeBetweenPrimaryWeaponShots;} }
    public float TimeBetweenSecondaryWeaponShots { get { return m_timeBetweenSecondaryWeaponShots;} }


    #region IContextDefinition

    public BehaviourContext MakeContext()
    {
        return new BehaviourContext(this);
    }

    #endregion // IContextDefinition
}

public class BehaviourContext : IContext
{
    private BehaviourDefinition m_definition;
    private float m_timeTilNextPrimaryShotAttempt;
    private float m_timeTilNextSecondaryShotAttempt;

    public BehaviourDefinition Definition { get { return m_definition; } }

    public bool CanShootPrimaryWeapon
    {
        get
        {
            return m_definition.CanUsePrimaryWeapon && m_timeTilNextPrimaryShotAttempt <= 0.0f;
        }
    }

    public bool CanShootSecondaryWeapon
    {
        get
        {
            return m_definition.CanUseSecondaryWeapon && m_timeTilNextSecondaryShotAttempt <= 0.0f;
        }
    }

    public BehaviourContext(BehaviourDefinition a_definition)
    {
        m_definition = a_definition;
    }

    public void OnPrimaryWeaponUsed()
    {
        m_timeTilNextPrimaryShotAttempt = m_definition.TimeBetweenPrimaryWeaponShots;
    }

    public void OnSecondaryWeaponUsed()
    {
        m_timeTilNextSecondaryShotAttempt = m_definition.TimeBetweenSecondaryWeaponShots;
    }

    #region IContext

    public void Process(float a_deltaTime)
    {
        m_timeTilNextPrimaryShotAttempt = Mathf.Max(m_timeTilNextPrimaryShotAttempt - a_deltaTime, 0.0f);
        m_timeTilNextSecondaryShotAttempt = Mathf.Max(m_timeTilNextSecondaryShotAttempt - a_deltaTime, 0.0f);
    }

    #endregion // IContext
}
