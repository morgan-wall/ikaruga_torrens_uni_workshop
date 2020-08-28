using UnityEngine;
using UnityEngine.UI;

public class EndLevelViewController : MonoBehaviour
{
    public delegate void RetryDelegate(EndLevelViewController a_endLevelViewController);
    public delegate void LevelSelectDelegate(EndLevelViewController a_endLevelViewController);

    private static readonly string k_victoryText = "VICTORY";
    private static readonly string k_defeatText = "DEFEAT";

    [SerializeField]
    private Text m_resultText = default;

    [SerializeField]
    private Text m_levelText = default;

    [SerializeField]
    private Text m_shipText = default;

    [SerializeField]
    private Text m_scoreText = default;

    [SerializeField]
    private Button m_retryButton = default;

    [SerializeField]
    private Button m_levelSelectButton = default;

    public event RetryDelegate OnRetryButtonClicked;
    public event LevelSelectDelegate OnLevelSelectButtonClicked;

    public void Init(bool a_victorious, string a_levelName, string a_shipName, int a_score)
    {
        m_resultText.text = a_victorious ? k_victoryText : k_defeatText;
        m_levelText.text = $"LEVEL - {a_levelName}";
        m_shipText.text = $"SHIP - {a_shipName}";
        m_scoreText.text = $"SCORE - {a_score}";
    }

    private void OnRetryButtonClick()
    {
        if (OnRetryButtonClicked != null)
        {
            OnRetryButtonClicked(this);
        }
    }

    private void OnLevelSelectButtonClick()
    {
        if (OnLevelSelectButtonClicked != null)
        {
            OnLevelSelectButtonClicked(this);
        }
    }

    private void Awake()
    {
        m_retryButton.onClick.AddListener(() => OnRetryButtonClick());
        m_levelSelectButton.onClick.AddListener(() => OnLevelSelectButtonClick());
    }
}
