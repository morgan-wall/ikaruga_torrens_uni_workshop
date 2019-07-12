using UnityEngine;
using UnityEngine.Assertions;

public class UICanvas : MonoBehaviour
{
    public delegate void StartSessionDelegate(UICanvas a_uiCanvas, LevelDefinition a_levelDefinition, ShipDefinition a_shipDefinition);

    public enum View
    {
        Title = 0,
        LevelSelect,
        ShipSelect,
        InGame,
        EndLevel,

        COUNT,
    }

    [SerializeField]
    private TitleViewController m_titleViewController;

    [SerializeField]
    private LevelSelectViewController m_levelSelectViewController;

    [SerializeField]
    private ShipSelectViewController m_shipSelectViewController;

    [SerializeField]
    private InGameViewController m_inGameViewController;

    [SerializeField]
    private EndLevelViewController m_endLevelViewController;

    private View m_activeView = View.Title;
    private GameObject[] m_viewControllersByView;
    private LevelDefinition m_selectedLevelDefinition;
    private ShipDefinition m_selectedShipDefinition;
    
    public event StartSessionDelegate OnSessionReady;

    public TitleViewController TitleViewController { get { return m_titleViewController; } }
    public LevelSelectViewController LevelSelectViewController { get { return m_levelSelectViewController; } }
    public ShipSelectViewController ShipSelectViewController { get { return m_shipSelectViewController; } }
    public InGameViewController InGameViewController { get { return m_inGameViewController; } }
    public EndLevelViewController EndLevelViewController { get { return m_endLevelViewController; } }

    public View ActiveView
    {
        get
        {
            return m_activeView;
        }
        set
        {
            if (m_activeView != value)
            {
                var previousView = m_activeView;
                m_activeView = value;
                OnViewExit(previousView);
                OnViewEnter(m_activeView);
            }
        }
    }

    private void OnViewEnter(View a_view)
    {
        m_viewControllersByView[(int)a_view].SetActive(true);
    }

    private void OnViewExit(View a_view)
    {
        m_viewControllersByView[(int)a_view].SetActive(false);
    }

    private void OnLevelSelectButtonClicked(TitleViewController a_titleViewController)
    {
        Assert.IsTrue(m_titleViewController == a_titleViewController);
        Assert.IsTrue(ActiveView == View.Title);
        ActiveView = View.LevelSelect;
    }

    private void OnLevelSelected(LevelSelectViewController a_levelSelectViewController, LevelDefinition a_levelDefinition)
    {
        Assert.IsNotNull(a_levelDefinition);
        Assert.IsTrue(m_levelSelectViewController == a_levelSelectViewController);
        Assert.IsTrue(ActiveView == View.LevelSelect);
        m_selectedLevelDefinition = a_levelDefinition;
        ActiveView = View.ShipSelect;
    }

    private void OnShipSelected(ShipSelectViewController a_shipSelectViewController, ShipDefinition a_shipDefinition)
    {
        Assert.IsNotNull(a_shipDefinition);
        Assert.IsTrue(m_shipSelectViewController == a_shipSelectViewController);
        Assert.IsTrue(ActiveView == View.ShipSelect);
        m_selectedShipDefinition = a_shipDefinition;
        ReadySession();
    }

    private void OnRetryButtonClicked(EndLevelViewController a_endLevelViewController)
    {
        Assert.IsTrue(m_endLevelViewController == a_endLevelViewController);
        Assert.IsTrue(ActiveView == View.EndLevel);
        ReadySession();
    }

    private void OnLevelSelectButtonClicked(EndLevelViewController a_endLevelViewController)
    {
        Assert.IsTrue(m_endLevelViewController == a_endLevelViewController);
        Assert.IsTrue(ActiveView == View.EndLevel);
        ActiveView = View.LevelSelect;
    }

    private void ReadySession()
    {
        if (OnSessionReady != null)
        {
            Assert.IsNotNull(m_selectedLevelDefinition);
            Assert.IsNotNull(m_selectedShipDefinition);
            OnSessionReady(this, m_selectedLevelDefinition, m_selectedShipDefinition);
        }
    }

    private void Awake()
    {
        m_viewControllersByView = new GameObject[]
        {
            m_titleViewController.gameObject,
            m_levelSelectViewController.gameObject,
            m_shipSelectViewController.gameObject,
            m_inGameViewController.gameObject,
            m_endLevelViewController.gameObject,
        };

        for (int i = 0; i < (int)View.COUNT; ++i)
        {
            var view = (View)i;
            if (view == ActiveView)
            {
                OnViewEnter(view);
            }
            else
            {
                OnViewExit(view);
            }
        }

        m_titleViewController.OnLevelSelectButtonClicked += OnLevelSelectButtonClicked;
        m_levelSelectViewController.OnLevelSelected += OnLevelSelected;
        m_shipSelectViewController.OnShipSelected += OnShipSelected;
        m_endLevelViewController.OnRetryButtonClicked += OnRetryButtonClicked;
        m_endLevelViewController.OnLevelSelectButtonClicked += OnLevelSelectButtonClicked;
    }
}
