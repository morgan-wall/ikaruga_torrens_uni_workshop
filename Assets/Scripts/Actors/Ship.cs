using UnityEngine;
using UnityEngine.Assertions;

public class Ship : MonoBehaviour, IPooledObject
{
    public delegate void ShipDelegate(Ship a_ship);

    [SerializeField]
    private Model m_modelPrefab = default;

    private Model m_model;
    private ShipContext m_context;

    public event ShipDelegate OnShipContextBound;
    public event ShipDelegate OnShipContextUnbound;

    public Model Model { get { return m_model; } }
    public ShipContext Context { get { return m_context; } }

    public static Ship Get(ShipTargetable a_shipTargetable)
    {
        return a_shipTargetable.GetComponent<Ship>();
    }

    public void Bind(ShipContext a_context)
    {
        Assert.IsNotNull(a_context);
        Assert.IsTrue(m_context != a_context);
        m_context = a_context;

        if (OnShipContextBound != null)
        {
            OnShipContextBound(this);
        }
    }

    public void Unbind()
    {
        Assert.IsNotNull(m_context);
        m_context = null;

        if (OnShipContextUnbound != null)
        {
            OnShipContextUnbound(this);
        }
    }

    private void Awake()
    {
        m_model = GameObject.Instantiate(m_modelPrefab, transform);
    }

    private void Update()
    {
        m_context.Process(Time.deltaTime);
    }


    #region IPooledObject

    public void OnClaimed()
    {
        ShipManager.Instance.Add(this);
    }

    public void OnRelinquished()
    {
        ShipManager.Instance.Remove(this);
        Unbind();
    }

    public void Relinquish()
    {
        ObjectPoolManager.Instance.Relinquish(m_context.Definition.Prefab.gameObject, gameObject);
    }

    #endregion // IPooledObject
}
