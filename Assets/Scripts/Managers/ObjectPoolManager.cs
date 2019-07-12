using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObjectPoolManager
{
    private struct ObjectPool
    {
        private class ObjectInfo
        {
            public GameObject GameObject;
            public IPooledObject[] PooledObjects;
        }

        private static readonly int k_expansionFactor = 2;

        private GameObject m_prefab;
        private GameObject m_parentForClaimedObjects;
        private List<ObjectInfo> m_claimedObjects;
        private List<ObjectInfo> m_unclaimedObjects;
        private List<IPooledObject> m_pooledObjectCache;

        public ObjectPool(Transform a_objectPoolsParent, GameObject a_prefab, int a_capacity)
        {
            m_prefab = a_prefab;
            m_claimedObjects = new List<ObjectInfo>(a_capacity);
            m_unclaimedObjects = new List<ObjectInfo>(a_capacity);
            m_pooledObjectCache = new List<IPooledObject>();

            m_parentForClaimedObjects = new GameObject($"ObjectPool_{m_prefab.gameObject.name}");
            m_parentForClaimedObjects.transform.SetParent(a_objectPoolsParent, false);

            for (int i = 0; i < a_capacity; ++i)
            {
                AddToUnclaimedPool();
            }
        }

        public GameObject Claim()
        {
            if (m_unclaimedObjects.Count <= 0)
            {
                Expand();
            }
            Assert.IsTrue(m_unclaimedObjects.Count > 0);

            // Process the claimed object
            int claimedObjectIndex = m_unclaimedObjects.Count - 1;
            var claimedObjectInfo = m_unclaimedObjects[claimedObjectIndex];
            claimedObjectInfo.GameObject.transform.SetParent(null, false);
            claimedObjectInfo.GameObject.SetActive(true);

            // Update object registration
            m_unclaimedObjects.RemoveAt(claimedObjectIndex);
            m_claimedObjects.Add(claimedObjectInfo);

            // Perform object-specific processing
            for (int i = 0; i < claimedObjectInfo.PooledObjects.Length; ++i)
            {
                claimedObjectInfo.PooledObjects[i].OnClaimed();
            }

            return claimedObjectInfo.GameObject;
        }

        public void Relinquish(GameObject a_gameObject)
        {
            // Find the specific game object
            int index = -1;
            for (int i = 0; i < m_claimedObjects.Count; ++i)
            {
                if (m_claimedObjects[i].GameObject == a_gameObject)
                {
                    index = i;
                    break;
                }
            }
            if (index < 0)
            {
                return;
            }

            // Update object registration
            var relinquishedObjectInfo = m_claimedObjects[index];
            m_claimedObjects.RemoveAt(index);
            m_unclaimedObjects.Add(relinquishedObjectInfo);

            // Process the relinquished object
            relinquishedObjectInfo.GameObject.transform.SetParent(m_parentForClaimedObjects.transform, false);
            relinquishedObjectInfo.GameObject.SetActive(false);

            // Perform object-specific processing
            for (int i = 0; i < relinquishedObjectInfo.PooledObjects.Length; ++i)
            {
                relinquishedObjectInfo.PooledObjects[i].OnRelinquished();
            }
        }

        private void AddToUnclaimedPool()
        {
            Assert.IsTrue(m_pooledObjectCache.Count <= 0);
            var instantiatedGameObject = GameObject.Instantiate(m_prefab, m_parentForClaimedObjects.transform);
            m_pooledObjectCache.AddRange(instantiatedGameObject.GetComponentsInChildren<IPooledObject>());
            instantiatedGameObject.SetActive(false);
            m_unclaimedObjects.Add(new ObjectInfo{ GameObject = instantiatedGameObject, PooledObjects = m_pooledObjectCache.ToArray() });
            m_pooledObjectCache.Clear();
        }

        private void Expand()
        {
            int existingCapacity = m_unclaimedObjects.Capacity;
            int expandedCapacity = existingCapacity * k_expansionFactor;

            m_claimedObjects.Capacity = expandedCapacity;
            m_unclaimedObjects.Capacity = expandedCapacity;

            int additionalObjects = expandedCapacity - existingCapacity;
            for (int i = 0; i < additionalObjects; ++i)
            {
                AddToUnclaimedPool();
            }
        }
    }

    private GameObject m_parentForObjectPools;
    private Dictionary<GameObject, ObjectPool> m_objectsForPrefab = new Dictionary<GameObject, ObjectPool>();

    private static ObjectPoolManager s_instance;

    public static ObjectPoolManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new ObjectPoolManager();
            }
            return s_instance;
        }
    }

    public ObjectPoolManager()
    {
        m_parentForObjectPools = new GameObject("ObjectPools");
    }

    public bool HasObjectPool(GameObject a_prefab)
    {
        return m_objectsForPrefab.ContainsKey(a_prefab);
    }

    public void AddObjectPool(GameObject a_prefab, int a_capacity)
    {
        Assert.IsTrue(!m_objectsForPrefab.ContainsKey(a_prefab));
        m_objectsForPrefab.Add(a_prefab, new ObjectPool(m_parentForObjectPools.transform, a_prefab, a_capacity));
    }

    public GameObject Claim(GameObject a_prefab)
    {
        Assert.IsTrue(m_objectsForPrefab.ContainsKey(a_prefab));
        return m_objectsForPrefab[a_prefab].Claim();
    }

    public void Relinquish(GameObject a_prefab, GameObject a_instance)
    {
        Assert.IsTrue(m_objectsForPrefab.ContainsKey(a_prefab));
        m_objectsForPrefab[a_prefab].Relinquish(a_instance);
    }
}
