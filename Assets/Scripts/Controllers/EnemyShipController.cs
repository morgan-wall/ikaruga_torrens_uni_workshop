using UnityEngine;
using UnityEngine.Assertions;

public class EnemyShipController : ShipController
{
    private static readonly string k_projectileLayerName = "EnemyProjectiles";
    private static int k_projectileLayer = -1;

    private PathContext m_pathContext;
    private BehaviourContext m_behaviourContext;
    private bool m_primaryWeaponCanShoot;
    private bool m_secondaryWeaponCanShoot;

    protected override int ProjectileLayer
    {
        get
        {
            if (k_projectileLayer < 0)
            {
                k_projectileLayer = LayerMask.NameToLayer(k_projectileLayerName);
            }
            return k_projectileLayer;
        }
    }

    public void Init()
    {
        m_pathContext.Process(0.0f);
        transform.position = m_pathContext.CurrentPosition;

        m_primaryWeaponCanShoot = Ship.Context.PrimaryWeaponContext.CanShoot;
        Ship.Context.PrimaryWeaponContext.OnCanShoot += OnPrimaryWeaponCanShoot;
        Ship.Context.PrimaryWeaponContext.OnCannotShoot += OnPrimaryWeaponCannotShoot;

        m_secondaryWeaponCanShoot = Ship.Context.SecondaryWeaponContext.CanShoot;
        Ship.Context.SecondaryWeaponContext.OnCanShoot += OnSecondaryWeaponCanShoot;
        Ship.Context.SecondaryWeaponContext.OnCannotShoot += OnSecondaryWeaponCannotShoot;
    }

    public void Bind(PathContext a_pathContext)
    {
        Assert.IsNotNull(a_pathContext);
        m_pathContext = a_pathContext;
        m_pathContext.OnDestroyable += OnPathDestroyable;
        transform.position = m_pathContext.CurrentPosition;
    }

    public void Bind(BehaviourContext a_behaviourContext)
    {
        Assert.IsNotNull(a_behaviourContext);
        m_behaviourContext = a_behaviourContext;
    }

    public void Unbind()
    {
        m_pathContext = null;
        m_behaviourContext = null;
    }

    private void UpdatePath()
    {
        if (m_pathContext == null)
        {
            return;
        }

        m_pathContext.Process(Time.deltaTime);
        transform.position = m_pathContext.CurrentPosition;
    }

    private void UpdateBehaviour()
    {
        if (m_behaviourContext == null)
        {
            return;
        }

        m_behaviourContext.Process(Time.deltaTime);

        if (m_primaryWeaponCanShoot
            && m_behaviourContext.CanShootPrimaryWeapon)
        {
            FirePrimaryWeapon();
        }

        if (m_secondaryWeaponCanShoot
            && m_behaviourContext.CanShootSecondaryWeapon)
        {
            FireSecondaryWeapon();
        }
    }

    private void OnPrimaryWeaponCanShoot(WeaponContext a_context)
    {
        m_primaryWeaponCanShoot = true;
    }

    private void OnPrimaryWeaponCannotShoot(WeaponContext a_context)
    {
        m_primaryWeaponCanShoot = false;
    }

    private void OnSecondaryWeaponCanShoot(WeaponContext a_context)
    {
        m_secondaryWeaponCanShoot = true;
    }

    private void OnSecondaryWeaponCannotShoot(WeaponContext a_context)
    {
        m_secondaryWeaponCanShoot = false;
    }

    private void OnPathDestroyable(PathContext a_pathContext)
    {
        Ship.Relinquish();
    }

    protected override void Awake()
    {
        base.Awake();
        gameObject.layer = LayerMask.NameToLayer("Enemies");
    }

    private void Update()
    {
        UpdatePath();
        UpdateBehaviour();
    }
}
