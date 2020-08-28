using UnityEngine;
using UnityEngine.Assertions;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private int m_dynamicAudioSourcePoolCapacity = 30;

    [SerializeField]
    private GameObject m_dynamicAudioSourcePrefab = default;

    private static EffectManager s_instance;

    public static EffectManager Instance { get { return s_instance; } }

    public GameObject DynamicAudioSourcePrefab { get { return m_dynamicAudioSourcePrefab; } }

    private void Awake()
    {
        Assert.IsNull(s_instance, "You can only have one effect manager in a scene at a time. Please find and remove all duplicates.");
        s_instance = this;

        ObjectPoolManager.Instance.AddObjectPool(m_dynamicAudioSourcePrefab.gameObject, m_dynamicAudioSourcePoolCapacity);
    }

    private void OnDestroy()
    {
        s_instance = null;
    }
}
