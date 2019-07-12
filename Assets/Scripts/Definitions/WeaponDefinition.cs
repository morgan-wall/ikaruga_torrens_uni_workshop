using UnityEngine;

[CreateAssetMenu(menuName = "Game/Definitions/Weapon")]
public class WeaponDefinition : ScriptableObject, IContextDefinition<WeaponContext>
{
    [SerializeField]
    private string m_name;

    [SerializeField]
    private WeaponKindDefinition m_weaponKindDefinition;

    [SerializeField]
    private ProjectileDefinition m_projectileDefinition;

    public string Name { get { return m_name; } }
    public WeaponKindDefinition WeaponKindDefinition { get { return m_weaponKindDefinition; } }
    public ProjectileDefinition ProjectileDefiniton { get { return m_projectileDefinition; } }


    #region IContextDefinition

    public WeaponContext MakeContext()
    {
        return new WeaponContext(this);
    }

    #endregion // IContextDefinition
}

public class WeaponContext : IContext
{
    public delegate void WeaponDelegate(WeaponContext a_context);

    private WeaponDefinition m_definition;
    private int m_ammo;
    private float m_warmupRemaining;
    private float m_cooldownRemaining;
    private bool m_couldShoot;

    public event WeaponDelegate OnCanShoot;
    public event WeaponDelegate OnCannotShoot;

    public WeaponDefinition Definition { get { return m_definition; } }

    public int Ammo
    {
        get
        {
            return m_ammo;
        }
        private set
        {
            if (m_ammo == value)
            {
                return;
            }

            m_ammo = Mathf.Clamp(value, m_definition.WeaponKindDefinition.MinAmmo, m_definition.WeaponKindDefinition.MaxAmmo);
        }
    }

    public bool CanShoot
    {
        get
        {
            return Ammo > 0
                && m_warmupRemaining <= 0.0f
                && m_cooldownRemaining <= 0.0f;
        }
    }

    public WeaponContext(WeaponDefinition a_definition)
    {
        m_definition = a_definition;
        Ammo = m_definition.WeaponKindDefinition.StartingAmmo;
    }

    public bool Shoot(Transform[] a_muzzles, int a_layer)
    {
        if (!CanShoot
            || a_muzzles.Length <= 0)
        {
            return false;
        }

        for (int i = 0; i < a_muzzles.Length; ++i)
        {
            var muzzle = a_muzzles[i];
            var projectile = m_definition.ProjectileDefiniton.MakeObject();
            Utils.SetLayerRecursively(projectile.gameObject, a_layer);
            projectile.transform.position = muzzle.position;
            projectile.Direction = muzzle.up;
        }

        --Ammo;
        m_cooldownRemaining = Mathf.Max(m_definition.WeaponKindDefinition.Cooldown, 0.0f);
        m_warmupRemaining = Mathf.Max(m_definition.WeaponKindDefinition.Warmup, 0.0f);

        if (!CanShoot)
        {
            if (m_couldShoot
                && OnCannotShoot != null)
            {
                OnCannotShoot(this);
            }

            m_couldShoot = false;
        }

        return true;
    }


    #region IContext

    public void Process(float a_deltaTime)
    {
        m_cooldownRemaining = Mathf.Max(m_cooldownRemaining - a_deltaTime, 0.0f);
        m_warmupRemaining = Mathf.Max(m_warmupRemaining - a_deltaTime, 0.0f);

        if (CanShoot)
        {
            if (!m_couldShoot
                && OnCanShoot != null)
            {
                OnCanShoot(this);
            }

            m_couldShoot = true;
        }
        else
        {
            if (m_couldShoot
                && OnCannotShoot != null)
            {
                OnCannotShoot(this);
            }

            m_couldShoot = false;
        }
    }

    #endregion // IContext
}
