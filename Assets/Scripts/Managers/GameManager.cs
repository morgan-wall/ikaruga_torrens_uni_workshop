using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    public delegate void SessionDelegate(GameManager a_gameManager);
    public delegate void SessionEndedDelegate(GameManager a_gameManager, bool a_isVictorious);

    private class SessionInfo
    {
        public LevelContext LevelContext;
        public ShipDefinition ShipDefinition;
        public bool Ended;
        public bool Victorious;
        public float StartTime;
        public float Runtime;
        public int NextLevelEventIndex;
    }

    [SerializeField]
    private UICanvas m_uiCanvas;

    private bool m_processDestroyedPlayerShip;
    private Ship m_activePlayerShip;
    private Targetable m_activePlayerShipTargetable;
    private SessionInfo m_previousSessionInfo;
    private SessionInfo m_currentSessionInfo;

    public event SessionDelegate OnSessionInitialised;
    public event SessionEndedDelegate OnSessionEnded;

    private static GameManager s_instance;
    public static GameManager Instance { get { return s_instance; } }

    public ShipDefinition PlayerShipDefinition { get { return m_currentSessionInfo != null ? m_currentSessionInfo.ShipDefinition : null; } }
    public LevelContext LevelContext { get { return m_currentSessionInfo != null ? m_currentSessionInfo.LevelContext : null; } }

    public void End()
    {
        if (m_currentSessionInfo == null
            || m_currentSessionInfo.Ended)
        {
            return;
        }
        m_currentSessionInfo.Ended = true;

        m_currentSessionInfo.Victorious = m_currentSessionInfo.LevelContext.Lives > 0;

        if (OnSessionEnded != null)
        {
            OnSessionEnded(this, m_currentSessionInfo.Victorious);
        }

        Clear();
    }

    private void OnPlayerShipDestroyedBy(Targetable a_target, Targetable a_source)
    {
        Assert.IsTrue(m_activePlayerShipTargetable == a_target);
        m_activePlayerShipTargetable.OnDestroyedBy -= OnPlayerShipDestroyedBy;
        --m_currentSessionInfo.LevelContext.Lives;
        m_processDestroyedPlayerShip = true;
    }

    private void SpawnPlayerShip()
    {
        Assert.IsNotNull(m_currentSessionInfo, "A session must be initialised before the player ship can be spawned.");

        // Initialise the player ship
        // N.B. Rely upon the bounding logic to pull the ship to the bottom of the screen.
        m_activePlayerShip = m_currentSessionInfo.ShipDefinition.MakeObject();
        Assert.IsNotNull(m_activePlayerShip.GetComponent<PlayerShipController>());
        Assert.IsNotNull(m_activePlayerShip.GetComponent<BoundSpriteToScreen>());
        m_activePlayerShip.transform.position = new Vector3(m_activePlayerShip.transform.position.x, -10000.0f, 0.0f);

        // Hook up ship event callbacks
        m_activePlayerShipTargetable = m_activePlayerShip.GetComponent<Targetable>();
        Assert.IsNotNull(m_activePlayerShipTargetable);
        m_activePlayerShipTargetable.OnDestroyedBy += OnPlayerShipDestroyedBy;
    }

    private void OnSessionReady(UICanvas a_uiCanvas, LevelDefinition a_levelDefinition, ShipDefinition a_shipDefinition)
    {
        Assert.IsTrue(m_uiCanvas == a_uiCanvas);
        Assert.IsNull(m_currentSessionInfo, "Attempted to initialise the session multiple times. The session manager can only be initialised once.");
        Assert.IsNotNull(a_levelDefinition, "Attempted to initialise the session with null level data.");
        Assert.IsNotNull(a_shipDefinition, "Attempted to initialise the session with null ship data.");
        
        m_uiCanvas.ActiveView = UICanvas.View.InGame;

        // Apply the nominated level data
        m_currentSessionInfo = new SessionInfo();
        m_currentSessionInfo.LevelContext = a_levelDefinition.MakeContext();

        // Initialise the session config
        m_currentSessionInfo.ShipDefinition = a_shipDefinition;
        m_currentSessionInfo.Ended = false;
        m_currentSessionInfo.Victorious = false;
        m_currentSessionInfo.StartTime = Time.time;
        m_currentSessionInfo.Runtime = 0.0f;
        m_currentSessionInfo.NextLevelEventIndex = 0;

        SpawnPlayerShip();

        if (OnSessionInitialised != null)
        {
            OnSessionInitialised(this);
        }
    }

    private void Clear()
    {
        ShipManager.Instance.RemoveAll();
        ProjectileManager.Instance.RemoveAll();

        // Migrate the active session info to the previous session info
        Assert.IsNotNull(m_currentSessionInfo);
        m_previousSessionInfo = m_currentSessionInfo;
        m_currentSessionInfo = null;

        // Destroy the player ship data
        m_activePlayerShip = null;
        m_activePlayerShipTargetable = null;

        // Transition to the end level view
        var levelContext = m_previousSessionInfo.LevelContext;
        m_uiCanvas.EndLevelViewController.Init(m_previousSessionInfo.Victorious, levelContext.Definition.Name, m_previousSessionInfo.ShipDefinition.Name, levelContext.Score);
        m_uiCanvas.ActiveView = UICanvas.View.EndLevel;
    }

    private void ProcessDestroyedPlayerShip()
    {
        // Ensure the current session has been initialised
        if (m_currentSessionInfo == null)
        {
            return;
        }

        if (m_processDestroyedPlayerShip)
        {
            m_processDestroyedPlayerShip = false;
            if (m_currentSessionInfo.LevelContext.Lives > 0)
            {
                SpawnPlayerShip();
            }
            else
            {
                End();
            }
        }
    }

    private void ProcessCurrentSession()
    {
        // Ensure the current session has been initialised
        if (m_currentSessionInfo == null)
        {
            return;
        }

        // Ensure the session is still running
        if (m_currentSessionInfo.Ended)
        {
            return;
        }

        // Ensure the session has started
        if (m_currentSessionInfo.StartTime >= Time.time)
        {
            return;
        }

        // Execute any available events
        // N.B. We check that the current session info is not null because an end event could purge the data.
        var levelContext = m_currentSessionInfo.LevelContext;
        m_currentSessionInfo.Runtime = Time.time - m_currentSessionInfo.StartTime;
        while (m_currentSessionInfo != null
            && m_currentSessionInfo.NextLevelEventIndex < levelContext.Definition.Events.Length
            && levelContext.Definition.Events[m_currentSessionInfo.NextLevelEventIndex].Time <= m_currentSessionInfo.Runtime)
        {
            levelContext.Definition.Events[m_currentSessionInfo.NextLevelEventIndex].Event.Execute(this);
            if (m_currentSessionInfo != null)
            {
                ++m_currentSessionInfo.NextLevelEventIndex;
            }
        }

        if (m_currentSessionInfo != null)
        {
            levelContext.Process(Time.deltaTime);
        }
    }

    private void Awake()
    {
        Assert.IsNull(s_instance, "You can only have one session manager in a scene at a time. Please find and remove all duplicates.");
        s_instance = this;

        m_uiCanvas.OnSessionReady += OnSessionReady;
    }

    private void OnDestroy()
    {
        s_instance = null;
    }

    private void Update()
    {
        ProcessDestroyedPlayerShip();
        ProcessCurrentSession();
    }
}