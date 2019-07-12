using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonEffects : MonoBehaviour
{
    [SerializeField]
    private EffectSet m_effectSetPrefab;

    [SerializeField]
    private string[] m_onClickIds;

    private Button m_button;
    private EffectSet m_effectSet;

    private void OnButtonClick()
    {
        if (m_effectSet == null)
        {
            return;
        }

        for (int i = 0; i < m_onClickIds.Length; ++i)
        {
            m_effectSet.Begin(m_onClickIds[i], null);
        }
    }

    private void Awake()
    {
        m_effectSet = GameObject.Instantiate(m_effectSetPrefab, transform);

        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(OnButtonClick);
    }
}
