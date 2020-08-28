using UnityEngine;
using UnityEngine.Assertions;

public class ManualLevelDefinitionManager : MonoBehaviour
{
    [SerializeField]
    private LevelDefinition[] m_levelDefinitions = default;

    private static ManualLevelDefinitionManager s_instance;

    public static ManualLevelDefinitionManager Instance { get { return s_instance; } }

    public LevelDefinition[] LevelDefinitions { get { return m_levelDefinitions; } }

    private void Awake()
    {
        Assert.IsNull(s_instance, "You can only have one ManualLevelDefinitionManager in a scene at a time. Please find and remove all duplicates.");
        s_instance = this;
    }

    private void OnDestroy()
    {
        s_instance = null;
    }
}
