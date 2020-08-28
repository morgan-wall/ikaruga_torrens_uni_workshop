using UnityEngine;

[CreateAssetMenu(menuName = "Game/Definitions/Ship Kind")]
public class ShipKindDefinition : ScriptableObject
{
    [SerializeField]
    private string m_name = default;

    [SerializeField]
    private float m_startingHealth = default;

    [SerializeField]
    private float[] m_maxHealthInUpgradeOrder = default;

    [SerializeField]
    private float[] m_maxSpeedInUpgradeOrder = default;

    [SerializeField]
    private Element m_initialShieldElement = default;

    [SerializeField]
    private float m_shieldRechargeDuration = default;

    [SerializeField]
    private WeaponDefinition[] m_primaryWeaponDefinitionsInUpgradeOrder = default;

    [SerializeField]
    private WeaponDefinition[] m_secondaryWeaponDefinitionsInUpgradeOrder = default;

    public string Name { get { return m_name; } }
    public float StartingHealth { get { return m_startingHealth; } }
    public float[] MaxHealthInUpgradeOrder { get { return m_maxHealthInUpgradeOrder; } }
    public float[] MaxSpeedInUpgradeOrder { get { return m_maxSpeedInUpgradeOrder; } }
    public Element InitialShieldElement { get { return m_initialShieldElement; } }
    public float ShieldRechargeDuration { get { return m_shieldRechargeDuration; } }
    public WeaponDefinition[] PrimaryWeaponDefinitionsInUpgradeOrder { get { return m_primaryWeaponDefinitionsInUpgradeOrder; } }
    public WeaponDefinition[] SecondaryWeaponDefinitionsInUpgradeOrder { get { return m_secondaryWeaponDefinitionsInUpgradeOrder; } }
}
