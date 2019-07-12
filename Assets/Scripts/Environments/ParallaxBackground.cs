using System;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Serializable]
    private struct EntityInfo
    {
        public SpriteRenderer Prefab;
        public int Count;
    }

    private static readonly float k_spawnBufferY = 1.0f;
    private static readonly Vector3 k_moveDirection = Vector3.down;

    [SerializeField]
    private EntityInfo[] m_entityInfo;

    [SerializeField]
    private float m_metresPerSecond = 1.0f;

    private Vector3 m_bounds;
    private List<GameObject>[] m_entitiesPerInfo;

    private static Vector3 GetSpawnPosition(Vector3 a_worldBounds, Vector3 a_entityBounds)
    {
        float x = UnityEngine.Random.Range(-a_worldBounds.x - (a_entityBounds.x / 2.0f), a_worldBounds.x + (a_entityBounds.x / 2.0f));
        float y = UnityEngine.Random.Range(a_worldBounds.y + a_entityBounds.y, a_worldBounds.y + a_entityBounds.y + k_spawnBufferY);
        return new Vector3(x, y, 0.0f);
    }

    private void Awake()
    {
        m_entitiesPerInfo = new List<GameObject>[m_entityInfo.Length];

        for (int i = 0; i < m_entityInfo.Length; ++i)
        {
            var entityInfo = m_entityInfo[i];
            m_entitiesPerInfo[i] = new List<GameObject>(entityInfo.Count);
            ObjectPoolManager.Instance.AddObjectPool(entityInfo.Prefab.gameObject, entityInfo.Count);
        }
    }

    private void Start()
    {
        m_bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0.0f));

        for (int i = 0; i < m_entityInfo.Length; ++i)
        {
            var entityInfo = m_entityInfo[i];
            for (int j = 0; j < entityInfo.Count; ++j)
            {
                var spawnPosition = ParallaxBackground.GetSpawnPosition(m_bounds, entityInfo.Prefab.bounds.size);
                spawnPosition.y = UnityEngine.Random.Range(-m_bounds.y, m_bounds.y);

                var entity = ObjectPoolManager.Instance.Claim(entityInfo.Prefab.gameObject);
                entity.transform.position = spawnPosition;
                entity.transform.parent = transform;
                m_entitiesPerInfo[i].Add(entity);
            }
        }
    }

    private void Update()
    {
        // Translate all entities
        var translation = k_moveDirection * m_metresPerSecond * Time.deltaTime;
        for (int i = 0; i < m_entityInfo.Length; ++i)
        {
            var entityInfo = m_entityInfo[i];
            var entities = m_entitiesPerInfo[i];
            for (int j = entities.Count - 1; j >= 0; --j)
            {
                var entity = entities[j];
                entity.transform.Translate(translation);
                if (entity.transform.position.y <= -m_bounds.y)
                {
                    entity.transform.position = ParallaxBackground.GetSpawnPosition(m_bounds, entityInfo.Prefab.bounds.size);
                }
            }
        }
    }
}
