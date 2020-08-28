using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Ship))]
public class Shield : MonoBehaviour
{
    private static readonly int k_objectPoolCapacity = 10;

    [Serializable]
    private struct VisualInfo
    {
        public Element Element;
        public GameObject Prefab;
    }

    [SerializeField]
    private VisualInfo[] m_visualInfo = default;

    [SerializeField]
    private Transform m_shieldParent = default;

    private Ship m_ship;
    private Element m_currElement;
    private GameObject m_activeElement;

    private Element CurrentElement
    {
        get
        {
            return m_currElement;
        }
        set
        {
            if (m_currElement != value)
            {
                var previousElement = m_currElement;
                m_currElement = value;
                OnElementChanged(m_currElement, previousElement);
            }
        }
    }

    private VisualInfo GetVisualInfo(Element a_element)
    {
        for (int i = 0; i < m_visualInfo.Length; ++i)
        {
            if (m_visualInfo[i].Element == a_element)
            {
                return m_visualInfo[i];
            }
        }

        throw new ArgumentException();
    }

    private void OnElementChanged(Element a_newElement, Element a_oldElement)
    {
        if (a_oldElement != Element.None)
        {
            ObjectPoolManager.Instance.Relinquish(GetVisualInfo(a_oldElement).Prefab, m_activeElement);
        }
        if (a_newElement != Element.None)
        {
            m_activeElement = ObjectPoolManager.Instance.Claim(GetVisualInfo(a_newElement).Prefab);
            m_activeElement.transform.SetParent(m_shieldParent, false);
        }
    }

    private void OnElementChanged(ShipContext a_context, Element a_newElement, Element a_oldElement)
    {
        CurrentElement = a_newElement;
    }

    private void OnShipContextBound(Ship a_ship)
    {
        Assert.IsTrue(m_ship == a_ship);
        CurrentElement = m_ship.Context.ShieldElement;
        m_ship.Context.OnShipShieldElementChanged += OnElementChanged;
    }

    private void OnShipContextUnbound(Ship a_ship)
    {
        Assert.IsTrue(m_ship == a_ship);
    }

    private void Awake()
    {
        m_ship = GetComponentInParent<Ship>();
        Assert.IsNotNull(m_ship);

        m_ship.OnShipContextBound += OnShipContextBound;
        m_ship.OnShipContextUnbound += OnShipContextUnbound;

        for (int i = 0; i < m_visualInfo.Length; ++i)
        {
            var visualInfo = m_visualInfo[i];
            if (visualInfo.Element != Element.None
                && !ObjectPoolManager.Instance.HasObjectPool(visualInfo.Prefab))
            {
                ObjectPoolManager.Instance.AddObjectPool(visualInfo.Prefab, k_objectPoolCapacity);
            }
        }
    }
}
