using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField]
    private Text m_levelNameText = default;

    [SerializeField]
    private Button m_button = default;

    private LevelDefinition m_levelDefinition;

    public LevelDefinition LevelDefinition { get { return m_levelDefinition; } }
    public Button Button { get { return m_button; } }

    public void Init(LevelDefinition a_levelDefinition)
    {
        m_levelDefinition = a_levelDefinition;
        m_levelNameText.text = a_levelDefinition.Name;
    }
}
