using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Targetable))]
public class Projectile : MonoBehaviour, IPooledObject
{
    [SerializeField]
    private Model m_modelPrefab = default;

    [SerializeField]
    private ProjectileEffects m_onHitEffects = default;

    [SerializeField]
    private AudioSource m_onInitAudioSource = default;

    private Vector2 m_direction;
    private Model m_model;
    private Targetable m_targetable;
    private ProjectileContext m_context;
    
    public Model Model { get { return m_model; } }
    public ProjectileContext Context { get { return m_context; } }

    public Vector2 Direction
    {
        get
        {
            return m_direction;
        }
        set
        {
            if (m_direction == value)
            {
                return;
            }

            m_direction = value.normalized;

            if (m_direction.sqrMagnitude > 0.001f)
            {
                transform.up = new Vector3(Direction.x, Direction.y, 0.0f);
            }
        }
    }

    public void Bind(ProjectileContext a_context)
    {
        m_context = a_context;
        m_context.OnProjectileLifetimeEnded += OnProjectileLifetimeEnded;
    }

    public void Unbind()
    {
        m_context.OnProjectileLifetimeEnded -= OnProjectileLifetimeEnded;
        m_context = null;
    }

    public void Init()
    {
        m_onInitAudioSource.Play();
    }

    private void OnProjectileLifetimeEnded(ProjectileContext a_context)
    {
        if (m_context == a_context)
        {
            ObjectPoolManager.Instance.Relinquish(m_context.Definition.Prefab.gameObject, gameObject);
        }
    }

    private void OnModelBecameInvisible(Model a_model)
    {
        // N.B. We're only interested in relinquishing the projectile if it's still active.
        //      This ensures that we aren't doing double clean up on termination.
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        Assert.IsTrue(m_model == a_model);
        ObjectPoolManager.Instance.Relinquish(m_context.Definition.Prefab.gameObject, gameObject);
    }

    private void Awake()
    {
        m_targetable = GetComponent<Targetable>();
        m_model = GameObject.Instantiate(m_modelPrefab, transform);
        m_model.OnModelBecameInvisible += OnModelBecameInvisible;
    }

    private void Start()
    {
        m_onHitEffects.Init(m_targetable);
    }

    private void OnTriggerEnter2D(Collider2D a_collider2D)
    {
        var targetable = a_collider2D.GetComponentInParent<Targetable>();
        if (targetable == null)
        {
            return;
        }

        // MW_TODO: Migrate this logic into a centralised location (as part of the targetable / combat effect systems)
        if (targetable.DefensiveElement != m_targetable.OffensiveElement)
        {
            CombatEffect.Apply(new ApplyContext { Source = m_targetable, Target = targetable }, m_onHitEffects);
        }
        Targetable.MarkForDestruction(m_targetable, targetable);
    }

    private void Update()
    {
        m_context.Process(Time.deltaTime);
        transform.Translate(new Vector3(Direction.x, Direction.y, 0.0f) * m_context.Definition.Speed * Time.deltaTime, Space.World);
    }


    #region IPooledObject

    public void OnClaimed()
    {
        ProjectileManager.Instance.Add(this);
    }

    public void OnRelinquished()
    {
        ProjectileManager.Instance.Remove(this);
        Unbind();
    }

    public void Relinquish()
    {
        ObjectPoolManager.Instance.Relinquish(m_context.Definition.Prefab.gameObject, gameObject);
    }

    #endregion // IPooledObject
}
