using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectViewController : MonoBehaviour
{
    public delegate void LevelSelectedDelegate(LevelSelectViewController a_levelSelectViewController, LevelDefinition a_levelDefinition);

    private static readonly int k_buttonObjectPoolSize = 10;
    
    [SerializeField]
    private LevelSelectButton m_levelButtonPrefab;

    [SerializeField]
    private RectTransform m_levelButtonArea;

    private List<LevelSelectButton> m_levelSelectButtons = new List<LevelSelectButton>();

    public event LevelSelectedDelegate OnLevelSelected;

    public void Init()
    {
        float totalContentSize = 0.0f;
        GridLayoutGroup areaLayout = m_levelButtonArea.GetComponent<GridLayoutGroup>();
        float verticalCellSize = areaLayout.cellSize.y + areaLayout.spacing.y;

        // Setup the level information
        var levelDefinitions = ManualLevelDefinitionManager.Instance.LevelDefinitions;
        for (int i = 0; i < levelDefinitions.Length; ++i)
        {
            LevelSelectButton button = ObjectPoolManager.Instance.Claim(m_levelButtonPrefab.gameObject).GetComponent<LevelSelectButton>();
            button.transform.SetParent(m_levelButtonArea, false);
            button.Init(levelDefinitions[i]);

            int tempButtonIndex = m_levelSelectButtons.Count;
            button.Button.onClick.AddListener(() => OnLevelSelectButtonClicked(tempButtonIndex));

            totalContentSize += verticalCellSize;

            m_levelSelectButtons.Add(button);
        }

        RectTransform contentTransform = m_levelButtonArea.GetComponent<RectTransform>();
        contentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalContentSize);
    }

    private void OnLevelSelectButtonClicked(int a_index)
    {
        if (OnLevelSelected != null)
        {
            OnLevelSelected(this, m_levelSelectButtons[a_index].LevelDefinition);
        }
    }

    private void Awake()
    {
        ObjectPoolManager.Instance.AddObjectPool(m_levelButtonPrefab.gameObject, k_buttonObjectPoolSize);
    }

    private void Start()
    {
        Init();
    }
}
