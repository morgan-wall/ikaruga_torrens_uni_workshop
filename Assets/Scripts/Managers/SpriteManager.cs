using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SpriteManager : MonoBehaviour
{
    [Serializable]
    private struct NumberInfo
    {
        public int Number;
        public GameObject Prefab;
    }

    private static readonly int k_pooledObjectCount = 10;
    private static readonly int k_decimalCharacterCount = 10;
    private static readonly int k_smallestDecimalDigit = 0;
    private static readonly int k_largestDecimalDigit = 9;

    private static SpriteManager s_instance;
    public static SpriteManager Instance { get { return s_instance; } }

    [SerializeField]
    private NumberInfo[] m_numberInfo = default;

    private GameObject[] m_prefabForDecimalDigit = new GameObject[k_decimalCharacterCount];

    public GameObject GetNumberPrefab(int a_number)
    {
        Assert.IsTrue(a_number >= k_smallestDecimalDigit && a_number <= k_largestDecimalDigit);
        return m_prefabForDecimalDigit[a_number];
    }

    private void Awake()
    {
        Assert.IsNull(s_instance, "You can only have one sprite manager in a scene at a time. Please find and remove all duplicates.");
        s_instance = this;

        Assert.IsTrue(m_numberInfo.Length == k_decimalCharacterCount);
        for (int i = 0; i < m_numberInfo.Length; ++i)
        {
            var numberInfo = m_numberInfo[i];
            Assert.IsTrue(numberInfo.Number >= k_smallestDecimalDigit && numberInfo.Number <= k_largestDecimalDigit);
            Assert.IsNull(m_prefabForDecimalDigit[numberInfo.Number]);
            ObjectPoolManager.Instance.AddObjectPool(numberInfo.Prefab, k_pooledObjectCount);
            m_prefabForDecimalDigit[numberInfo.Number] = numberInfo.Prefab;
        }

        for (int i = 0; i < m_prefabForDecimalDigit.Length; ++i)
        {
            if (m_prefabForDecimalDigit[i] == null)
            {
                throw new ArgumentException();
            }
        }
    }

    private void OnDestroy()
    {
        s_instance = null;
    }
}
