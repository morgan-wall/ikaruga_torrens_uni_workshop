using UnityEngine;
using UnityEngine.UI;

public class ShipSelectButton : MonoBehaviour
{
    [SerializeField]
    private Text m_shipNameText;

    [SerializeField]
    private Button m_button;

    private ShipDefinition m_shipDefinition;

    public ShipDefinition ShipDefinition { get { return m_shipDefinition; } }
    public Button Button { get { return m_button; } }

    public void Init(ShipDefinition a_shipDefinition)
    {
        m_shipDefinition = a_shipDefinition;
        m_shipNameText.text = m_shipDefinition.Name;
    }
}
