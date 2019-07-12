using UnityEngine;

public class EffectSet : MonoBehaviour
{
    private Effect[] m_effects;

    public void Begin(string a_id, Model a_model)
    {
        for (int i = 0; i < m_effects.Length; ++i)
        {
            var effect = m_effects[i];
            if (effect.BeginId == a_id)
            {
                effect.Begin(a_model);
            }
        }
    }

    public void End(string a_id, Model a_model)
    {
        for (int i = 0; i < m_effects.Length; ++i)
        {
            var effect = m_effects[i];
            if (effect.EndId == a_id)
            {
                effect.End(a_model);
            }
        }
    }

    private void Awake()
    {
        m_effects = transform.GetComponentsInDirectChildren<Effect>();
    }
}
