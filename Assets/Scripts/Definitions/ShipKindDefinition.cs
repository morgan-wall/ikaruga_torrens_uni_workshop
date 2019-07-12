using UnityEngine;

[CreateAssetMenu(menuName = "Game/Definitions/Ship Kind")]
public class ShipKindDefinition : ScriptableObject
{
    [SerializeField]
    private string m_name;

    [SerializeField]
    private float m_startingHealth;

    [SerializeField]
    private float[] m_maxHealthInUpgradeOrder;

    [SerializeField]
    private float[] m_maxSpeedInUpgradeOrder;

    [SerializeField]
    private Element m_initialShieldElement;

    [SerializeField]
    private float m_shieldRechargeDuration;

    [SerializeField]
    private WeaponDefinition[] m_primaryWeaponDefinitionsInUpgradeOrder;

    [SerializeField]
    private WeaponDefinition[] m_secondaryWeaponDefinitionsInUpgradeOrder;

    public string Name { get { return m_name; } }
    public float StartingHealth { get { return m_startingHealth; } }
    public float[] MaxHealthInUpgradeOrder { get { return m_maxHealthInUpgradeOrder; } }
    public float[] MaxSpeedInUpgradeOrder { get { return m_maxSpeedInUpgradeOrder; } }
    public Element InitialShieldElement { get { return m_initialShieldElement; } }
    public float ShieldRechargeDuration { get { return m_shieldRechargeDuration; } }
    public WeaponDefinition[] PrimaryWeaponDefinitionsInUpgradeOrder { get { return m_primaryWeaponDefinitionsInUpgradeOrder; } }
    public WeaponDefinition[] SecondaryWeaponDefinitionsInUpgradeOrder { get { return m_secondaryWeaponDefinitionsInUpgradeOrder; } }
}
