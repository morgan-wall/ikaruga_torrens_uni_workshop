using UnityEngine;

[CreateAssetMenu(menuName = "Game/Definitions/Projectile")]
public class ProjectileDefinition : ScriptableObject, IContextDefinition<ProjectileContext>, IObjectDefinition<Projectile>
{
    private static readonly int k_objectPoolSize = 30;

    [SerializeField]
    private string m_name;

    [SerializeField]
    private float m_speed;

    [SerializeField]
    private float m_lifetime = -1.0f;

    [SerializeField]
    private Element m_element;

    [SerializeField]
    private Projectile m_prefab;

    public string Name { get { return m_name; } }
    public float Speed { get { return m_speed; } }
    public float Lifetime { get { return m_lifetime; } }
    public Element Element { get { return m_element; } }
    public Projectile Prefab { get { return m_prefab; } }


    #region IContextDefinition

    public ProjectileContext MakeContext()
    {
        return new ProjectileContext(this);
    }

    #endregion // IContextDefinition


    #region IObjectDefinition

    public Projectile MakeObject()
    {
        if (!ObjectPoolManager.Instance.HasObjectPool(m_prefab.gameObject))
        {
            ObjectPoolManager.Instance.AddObjectPool(m_prefab.gameObject, k_objectPoolSize);
        }

        var projectile = ObjectPoolManager.Instance.Claim(m_prefab.gameObject).GetComponent<Projectile>();
        projectile.Bind(MakeContext());
        projectile.Init();
        return projectile;
    }

    #endregion // IObjectDefinition
}

public class ProjectileContext : IContext
{
    public delegate void ProjectileDelegate(ProjectileContext a_context);

    private ProjectileDefinition m_definition;
    private bool m_hasLifetime;
    private float m_timeRemaining;
    
    public ProjectileDefinition Definition { get { return m_definition; } }
    public float TimeRemaining { get { return m_timeRemaining; } }

    public event ProjectileDelegate OnProjectileLifetimeEnded;

    public ProjectileContext(ProjectileDefinition a_definition)
    {
        m_definition = a_definition;
        m_hasLifetime = m_definition.Lifetime > 0.0f;
        m_timeRemaining = m_definition.Lifetime;
    }


    #region IContext

    public void Process(float a_deltaTime)
    {
        if (!m_hasLifetime)
        {
            return;
        }

        float previousTimeRemaining = m_timeRemaining;
        m_timeRemaining = Mathf.Max(0.0f, m_timeRemaining - a_deltaTime);
        if (previousTimeRemaining <= 0.0f
            || m_timeRemaining > 0.0f)
        {
            return;
        }

        if (OnProjectileLifetimeEnded != null)
        {
            OnProjectileLifetimeEnded(this);
        }
    }

    #endregion // IContext
}
