using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

[CreateAssetMenu(menuName = "Game/Definitions/Ship")]
public class ShipDefinition : ScriptableObject, IContextDefinition<ShipContext>, IObjectDefinition<Ship>
{
    private static readonly int k_objectPoolSize = 15;

    [SerializeField]
    private string m_name;

    [SerializeField]
    private ShipKindDefinition m_shipKindDefinition;

    [SerializeField]
    private Image m_iconPrefab;

    [SerializeField]
    private Ship m_prefab;

    public string Name { get { return m_name; } }
    public ShipKindDefinition ShipKindDefinition { get { return m_shipKindDefinition; } }
    public Image IconPrefab { get { return m_iconPrefab; } }
    public Ship Prefab { get { return m_prefab; } }


    #region IContextDefinition

    public ShipContext MakeContext()
    {
        return new ShipContext(this);
    }

    #endregion // IContextDefinition


    #region IObjectDefinition

    public Ship MakeObject()
    {
        if (!ObjectPoolManager.Instance.HasObjectPool(m_prefab.gameObject))
        {
            ObjectPoolManager.Instance.AddObjectPool(m_prefab.gameObject, k_objectPoolSize);
        }

        var ship = ObjectPoolManager.Instance.Claim(m_prefab.gameObject).GetComponent<Ship>();
        ship.Bind(MakeContext());
        return ship;
    }

    #endregion // IObjectDefinition
}

public class ShipContext : IContext
{
    public delegate void ShipShieldDelegate(ShipContext a_context);
    public delegate void ShipShieldChangedDelegate(ShipContext a_context, Element a_newElement, Element a_oldElement);

    private ShipDefinition m_definition;
    private int m_maxHealthIndex;
    private float m_health;
    private int m_maxSpeedIndex;
    private Element m_shieldElement;
    private Element m_shieldElementOnDeactivate;
    private bool m_shieldActive;
    private float m_timeTilShieldReactivate;
    private int m_primaryWeaponIndex;
    private WeaponContext m_primaryWeaponContext;
    private int m_secondaryWeaponIndex;
    private WeaponContext m_secondaryWeaponContext;

    public event ShipShieldDelegate OnShipShieldActivated;
    public event ShipShieldDelegate OnShipShieldDeactivated;
    public event ShipShieldChangedDelegate OnShipShieldElementChanged;

    public ShipDefinition Definition { get { return m_definition; } }
    public WeaponContext PrimaryWeaponContext { get { return m_primaryWeaponContext; } }
    public WeaponContext SecondaryWeaponContext { get { return m_secondaryWeaponContext; } }

    public float Health
    {
        get
        {
            return m_health;
        }
        set
        {
            m_health = Mathf.Clamp(value, 0.0f, m_definition.ShipKindDefinition.MaxHealthInUpgradeOrder[m_maxHealthIndex]);
        }
    }

    public float Speed
    {
        get
        {
            return m_definition.ShipKindDefinition.MaxSpeedInUpgradeOrder[m_maxSpeedIndex];
        }
    }

    public Element ShieldElement
    {
        get
        {
            return m_shieldElement;
        }
        set
        {
            if (m_shieldActive
                && m_shieldElement != value)
            {
                var previousElement = m_shieldElement;
                m_shieldElement = value;
                if (OnShipShieldElementChanged != null)
                {
                    OnShipShieldElementChanged(this, m_shieldElement, previousElement);
                }
            }
        }
    }

    public bool ShieldActive
    {
        get
        {
            return m_shieldActive;
        }
        set
        {
            if (m_shieldActive != value)
            {
                if (!value)
                {
                    m_shieldElementOnDeactivate = ShieldElement;
                    ShieldElement = Element.None;
                }

                m_shieldActive = value;

                if (m_shieldActive)
                {
                    ShieldElement = m_shieldElementOnDeactivate;

                    if (OnShipShieldActivated != null)
                    {
                        OnShipShieldActivated(this);
                    }
                }
                else
                {
                    if (OnShipShieldDeactivated != null)
                    {
                        OnShipShieldDeactivated(this);
                    }
                }
            }
        }
    }

    private static void ApplyStatUpgradeDelta(int a_delta, ref int a_index, ref float a_stat, float[] a_upgradeDefinitions)
    {
        int newIndex = Mathf.Clamp(a_index + a_delta, 0, a_upgradeDefinitions.Length - 1);
        if (newIndex == a_index)
        {
            return;
        }

        a_index = newIndex;
        a_stat = Mathf.Clamp(a_stat, 0.0f, a_upgradeDefinitions[a_index]);
    }

    private static void ApplyWeaponUpgradeDelta(int a_delta, ref int a_index, ref WeaponContext a_weaponContext, WeaponDefinition[] a_upgradeDefinitions)
    {
        int newIndex = Mathf.Clamp(a_index + a_delta, 0, a_upgradeDefinitions.Length - 1);
        if (newIndex == a_index)
        {
            return;
        }

        a_index = newIndex;
        a_weaponContext = a_upgradeDefinitions[a_index].MakeContext() as WeaponContext;
    }

    public ShipContext(ShipDefinition a_definition)
    {
        m_definition = a_definition;
        
        m_shieldActive = true;
        m_shieldElement = a_definition.ShipKindDefinition.InitialShieldElement;

        Assert.IsTrue(m_definition.ShipKindDefinition.MaxHealthInUpgradeOrder.Length > 0);
        m_health = m_definition.ShipKindDefinition.MaxHealthInUpgradeOrder[0];

        Assert.IsTrue(m_definition.ShipKindDefinition.PrimaryWeaponDefinitionsInUpgradeOrder.Length > 0);
        m_primaryWeaponContext = m_definition.ShipKindDefinition.PrimaryWeaponDefinitionsInUpgradeOrder[0].MakeContext() as WeaponContext;

        Assert.IsTrue(m_definition.ShipKindDefinition.SecondaryWeaponDefinitionsInUpgradeOrder.Length > 0);
        m_secondaryWeaponContext = m_definition.ShipKindDefinition.SecondaryWeaponDefinitionsInUpgradeOrder[0].MakeContext() as WeaponContext;
    }

    public void DeactivateShield()
    {
        ShieldActive = false;
        m_timeTilShieldReactivate = Time.time + m_definition.ShipKindDefinition.ShieldRechargeDuration;
    }

    public void ApplyHealthUpgradeDelta(int a_delta)
    {
        ApplyStatUpgradeDelta(a_delta, ref m_maxHealthIndex, ref m_health, m_definition.ShipKindDefinition.MaxHealthInUpgradeOrder);
    }

    public void ApplySpeedUpgradeDelta(int a_delta)
    {
        int newIndex = Mathf.Clamp(m_maxSpeedIndex + a_delta, 0, m_definition.ShipKindDefinition.MaxSpeedInUpgradeOrder.Length - 1);
        if (newIndex == m_maxSpeedIndex)
        {
            return;
        }

        m_maxSpeedIndex = newIndex;
    }

    public void ApplyPrimaryWeaponUpgradeDelta(int a_delta)
    {
        ApplyWeaponUpgradeDelta(a_delta, ref m_primaryWeaponIndex, ref m_primaryWeaponContext, m_definition.ShipKindDefinition.PrimaryWeaponDefinitionsInUpgradeOrder);
    }

    public void ApplySecondaryWeaponUpgradeDelta(int a_delta)
    {
        ApplyWeaponUpgradeDelta(a_delta, ref m_secondaryWeaponIndex, ref m_secondaryWeaponContext, m_definition.ShipKindDefinition.SecondaryWeaponDefinitionsInUpgradeOrder);
    }


    #region IContext

    public void Process(float a_deltaTime)
    {
        m_primaryWeaponContext.Process(a_deltaTime);
        m_secondaryWeaponContext.Process(a_deltaTime);

        if (!ShieldActive
            && Time.time >= m_timeTilShieldReactivate)
        {
            ShieldActive = true;
        }
    }

    #endregion // IContext
}
