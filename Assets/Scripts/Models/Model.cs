using System;
using UnityEngine;

public class Model : MonoBehaviour
{
    public delegate void ModelDelegate(Model a_model);

    [Serializable]
    public struct WeaponConfig
    {
        public string Name;
        public Transform[] Muzzles;
    }

    [SerializeField]
    private WeaponConfig[] m_weaponConfigurations;

    private Transform m_dynamicEffectAttachment;

    public event ModelDelegate OnModelBecameInvisible;
    public event ModelDelegate OnModelBecameVisible;

    public Transform DynamicEffectAttachment { get { return m_dynamicEffectAttachment; } }
    public WeaponConfig[] WeaponConfigurations { get { return m_weaponConfigurations; } }

    public static Model Get(Targetable a_targetable)
    {
        return a_targetable.GetComponentInChildren<Model>();
    }

    public int GetWeaponConfigurationIndex(string a_name)
    {
        for (int i = 0; i < m_weaponConfigurations.Length; ++i)
        {
            var weaponConfig = m_weaponConfigurations[i];
            if (weaponConfig.Name == a_name)
            {
                return i;
            }
        }
        return -1;
    }

    private void Awake()
    {
        m_dynamicEffectAttachment = new GameObject("dynamic_effect_attachment").transform;
        m_dynamicEffectAttachment.parent = transform;
        m_dynamicEffectAttachment.localPosition = Vector3.zero;
    }

    private void OnDisable()
    {
        DynamicEffectAttachment.DetachChildren();
    }

    private void OnBecameInvisible()
    {
        if (OnModelBecameInvisible != null)
        {
            OnModelBecameInvisible(this);
        }
    }

    private void OnBecameVisible()
    {
        if (OnModelBecameVisible != null)
        {
            OnModelBecameVisible(this);
        }
    }
}
