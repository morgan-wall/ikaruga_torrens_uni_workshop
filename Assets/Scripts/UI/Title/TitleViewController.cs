using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TitleViewController : MonoBehaviour
{
    public delegate void LevelSelectDelegate(TitleViewController a_titleViewController);

    [SerializeField]
    private Button m_levelSelectButton;

    public event LevelSelectDelegate OnLevelSelectButtonClicked;

    private void OnLevelSelectButtonClick()
    {
        if (OnLevelSelectButtonClicked != null)
        {
            OnLevelSelectButtonClicked(this);
        }
    }

    private void Awake()
    {
        Assert.IsNotNull(m_levelSelectButton);
        m_levelSelectButton.onClick.AddListener(() => OnLevelSelectButtonClick());
    }
}
