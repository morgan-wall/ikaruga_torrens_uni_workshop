using UnityEngine;

[CreateAssetMenu(menuName = "Game/Definitions/Weapon Kind")]
public class WeaponKindDefinition : ScriptableObject
{
    [SerializeField]
    private string m_name = default;

    [SerializeField]
    private string m_ConfigName = default;

    [SerializeField]
    private int m_startingAmmo = default;

    [SerializeField]
    private int m_minAmmo = default;

    [SerializeField]
    private int m_maxAmmo = default;

    [SerializeField]
    private float m_cooldown = default;

    [SerializeField]
    private float m_warmup = default;

    public string Name { get { return m_name; } }
    public string ConfigName { get { return m_ConfigName; } }
    public int StartingAmmo { get { return m_startingAmmo; } }
    public int MinAmmo { get { return m_minAmmo; } }
    public int MaxAmmo { get { return m_maxAmmo; } }
    public float Cooldown { get { return m_cooldown; } }
    public float Warmup { get { return m_warmup; } }
}
