using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Definitions/Level")]
public class LevelDefinition : ScriptableObject, IContextDefinition<LevelContext>
{
    [Serializable]
    public struct TimedEvents
    {
        public Event Event;
        public float Time;
    }

    [SerializeField]
    private string m_name;

    [SerializeField]
    private int m_startingLives;

    [SerializeField]
    private int m_maxLives;

    [SerializeField]
    private TimedEvents[] m_events;

    public string Name { get { return m_name; } }
    public int StartingLives { get { return m_startingLives; } }
    public int MaxLives { get { return m_maxLives; } }
    public TimedEvents[] Events { get { return m_events; } }


    #region IContextDefinition

    public LevelContext MakeContext()
    {
        return new LevelContext(this);
    }

    #endregion // IContextDefinition
}


public class LevelContext : IContext
{
    public delegate void LivesChangedDelegate(LevelContext a_context, int a_newLives, int a_oldLives);
    public delegate void ScoreChangedDelegate(LevelContext a_context, int a_newScore, int a_oldScore);

    private LevelDefinition m_definition;
    private int m_lives;
    private int m_score;

    public event LivesChangedDelegate OnLivesChanged;
    public event ScoreChangedDelegate OnScoreChanged;

    public LevelDefinition Definition { get { return m_definition;} }

    public int Lives
    {
        get
        {
            return m_lives;
        }
        set
        {
            int boundedValue = Mathf.Clamp(value, 0, m_definition.MaxLives);
            if (m_lives != boundedValue)
            {
                int previousLives = m_lives;
                m_lives = boundedValue;
                if (OnLivesChanged != null)
                {
                    OnLivesChanged(this, m_lives, previousLives);
                }
            }
        }
    }

    public int Score
    {
        get
        {
            return m_score;
        }
        set
        {
            int boundedScore = Mathf.Max(value, 0);
            if (m_score != boundedScore)
            {
                int previousScore = m_score;
                m_score = boundedScore;
                if (OnScoreChanged != null)
                {
                    OnScoreChanged(this, m_score, previousScore);
                }
            } 
        }
    }

    public LevelContext(LevelDefinition a_definition)
    {
        m_definition = a_definition;
        Lives = m_definition.StartingLives;
    }


    #region IContext

    public void Process(float a_deltaTime)
    {
    }

    #endregion // IContext
}
