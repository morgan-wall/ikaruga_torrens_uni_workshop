using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ScoreViewController : MonoBehaviour
{
    private struct NumberInfo
    {
        public GameObject Prefab;
        public GameObject Instance;
    }

    [SerializeField]
    private RectTransform m_numbersGrid;

    private int m_score;
    private LevelContext m_boundedContext;
    private List<int> m_decimalDigitCache = new List<int>();
    private List<NumberInfo> m_numbersInGrid = new List<NumberInfo>();

    private int Score
    {
        get
        {
            return m_score;
        }
        set
        {
            if (m_score != value)
            {
                Assert.IsTrue(value >= 0);
                m_score = value;
                OnScoreChanged();
            }
        }
    }

    private void OnScoreChanged()
    {
        ClearNumbersInGrid();

        int processedScore = Score;
        while (processedScore > 0)
        {
            m_decimalDigitCache.Add(processedScore % 10);
            processedScore = processedScore / 10;
        }

        if (m_decimalDigitCache.Count <= 0)
        {
            m_decimalDigitCache.Add(0);
        }

        for (int i = m_decimalDigitCache.Count - 1; i >= 0; --i)
        {
            AddNumberToGrid(m_decimalDigitCache[i]);
        }
        m_decimalDigitCache.Clear();
    }

    private void OnScoreChanged(LevelContext a_context, int a_newScore, int a_oldScore)
    {
        if (m_boundedContext == a_context)
        {
            Score = a_newScore;
        }
    }

    private void AddNumberToGrid(int a_decimalDigit)
    {
        var decimalDigitPrefab = SpriteManager.Instance.GetNumberPrefab(a_decimalDigit);
        var decimalDigitInstance = ObjectPoolManager.Instance.Claim(decimalDigitPrefab);
        m_numbersInGrid.Add(new NumberInfo { Prefab = decimalDigitPrefab, Instance = decimalDigitInstance });
        decimalDigitInstance.transform.SetParent(m_numbersGrid, false);
        decimalDigitInstance.transform.SetAsLastSibling();
    }

    private void ClearNumbersInGrid()
    {
        for (int i = m_numbersInGrid.Count - 1; i >= 0; --i)
        {
            var numberInfo = m_numbersInGrid[i];
            numberInfo.Instance.transform.SetParent(null, false);
            ObjectPoolManager.Instance.Relinquish(numberInfo.Prefab, numberInfo.Instance);
        }
        m_numbersInGrid.Clear();
    }

    private void OnSessionInitialised(GameManager a_sessionManager)
    {
        Assert.IsNotNull(a_sessionManager.LevelContext);
        m_boundedContext = a_sessionManager.LevelContext;
        m_boundedContext.OnScoreChanged += OnScoreChanged;
        Score = m_boundedContext.Score;
        OnScoreChanged();
    }

    private void Start()
    {
        var sessionManager = GameManager.Instance;
        Assert.IsNotNull(sessionManager);
        GameManager.Instance.OnSessionInitialised += OnSessionInitialised;
        if (sessionManager.LevelContext != null)
        {
            OnSessionInitialised(sessionManager);
        }
    }
}
