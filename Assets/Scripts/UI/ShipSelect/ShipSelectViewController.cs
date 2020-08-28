using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectViewController : MonoBehaviour
{
    public delegate void ShipSelectedDelegate(ShipSelectViewController a_shipSelectViewController, ShipDefinition a_shipDefinition);

    private static readonly int k_buttonObjectPoolSize = 10;

    [SerializeField]
    private ShipSelectButton m_shipButtonPrefab = default;

    [SerializeField]
    private RectTransform m_shipButtonArea = default;

    private List<ShipSelectButton> m_shipSelectButtons = new List<ShipSelectButton>();

    public event ShipSelectedDelegate OnShipSelected;

    public void Init()
    {
        float totalContentSize = 0.0f;
        GridLayoutGroup areaLayout = m_shipButtonArea.GetComponent<GridLayoutGroup>();
        float verticalCellSize = areaLayout.cellSize.y + areaLayout.spacing.y;

        // Setup the level information
        var shipDefinitions = ManualShipDefinitionManager.Instance.ShipDefinitions;
        for (int i = 0; i < shipDefinitions.Length; ++i)
        {
            ShipSelectButton button = ObjectPoolManager.Instance.Claim(m_shipButtonPrefab.gameObject).GetComponent<ShipSelectButton>();
            button.transform.SetParent(m_shipButtonArea, false);
            button.Init(shipDefinitions[i]);

            int tempButtonIndex = m_shipSelectButtons.Count;
            button.Button.onClick.AddListener(() => OnLevelSelectButtonClicked(tempButtonIndex));

            totalContentSize += verticalCellSize;

            m_shipSelectButtons.Add(button);
        }

        RectTransform contentTransform = m_shipButtonArea.GetComponent<RectTransform>();
        contentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalContentSize);
    }

    private void OnLevelSelectButtonClicked(int a_index)
    {
        if (OnShipSelected != null)
        {
            OnShipSelected(this, m_shipSelectButtons[a_index].ShipDefinition);
        }
    }

    private void Awake()
    {
        ObjectPoolManager.Instance.AddObjectPool(m_shipButtonPrefab.gameObject, k_buttonObjectPoolSize);
    }

    private void Start()
    {
        Init();
    }
}
