using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class LivesViewController : MonoBehaviour
{
    private struct NumberInfo
    {
        public GameObject Prefab;
        public GameObject Instance;
    }

    [SerializeField]
    private RectTransform m_shipIconArea = default;

    [SerializeField]
    private RectTransform m_numbersGrid = default;

    private int m_lives;
    private GameObject m_shipIcon;
    private LevelContext m_boundedContext;
    private List<int> m_decimalDigitCache = new List<int>();
    private List<NumberInfo> m_numbersInGrid = new List<NumberInfo>();

    private int Lives
    {
        get
        {
            return m_lives;
        }
        set
        {
            if (m_lives != value)
            {
                Assert.IsTrue(value >= 0);
                m_lives = value;
                OnLivesChanged();
            }
        }
    }

    private void OnLivesChanged()
    {
        ClearNumbersInGrid();

        int processedLives = Lives;
        while (processedLives > 0)
        {
            m_decimalDigitCache.Add(processedLives % 10);
            processedLives = processedLives / 10;
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

    private void OnLivesChanged(LevelContext a_context, int a_newLives, int a_oldLives)
    {
        if (m_boundedContext == a_context)
        {
            Lives = a_newLives;
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

    private void OnSessionInitialised(GameManager a_gameManager)
    {
        // Register the level context information
        Assert.IsNotNull(a_gameManager.LevelContext);
        m_boundedContext = a_gameManager.LevelContext;
        m_boundedContext.OnLivesChanged += OnLivesChanged;

        // Refresh the ship icon
        // MW_TODO: Why aren't you using the object pool here? Fix this crap when you have time.
        if (m_shipIcon != null)
        {
            GameObject.Destroy(m_shipIcon.gameObject);
            m_shipIcon = null;
        }

        m_shipIcon = GameObject.Instantiate(a_gameManager.PlayerShipDefinition.IconPrefab.gameObject);
        m_shipIcon.transform.SetParent(m_shipIconArea, false);

        // Update the lives
        Lives = m_boundedContext.Lives;
        OnLivesChanged();
    }

    private void Start()
    {
        var gameManager = GameManager.Instance;
        Assert.IsNotNull(gameManager);
        GameManager.Instance.OnSessionInitialised += OnSessionInitialised;
        if (gameManager.LevelContext != null)
        {
            OnSessionInitialised(gameManager);
        }
    }
}
