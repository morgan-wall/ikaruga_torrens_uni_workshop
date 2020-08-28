using UnityEngine;
using UnityEngine.Assertions;

public class ManualShipDefinitionManager : MonoBehaviour
{
    [SerializeField]
    private ShipDefinition[] m_shipDefinitions = default;

    private static ManualShipDefinitionManager s_instance;

    public static ManualShipDefinitionManager Instance { get { return s_instance; } }

    public ShipDefinition[] ShipDefinitions { get { return m_shipDefinitions; } }

    private void Awake()
    {
        Assert.IsNull(s_instance, "You can only have one ManualShipDefinitionManager in a scene at a time. Please find and remove all duplicates.");
        s_instance = this;
    }

    private void OnDestroy()
    {
        s_instance = null;
    }
}
