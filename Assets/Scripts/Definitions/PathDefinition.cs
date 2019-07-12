using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(menuName = "Game/Definitions/Path")]
public class PathDefinition : ScriptableObject, IContextDefinition<PathContext>
{
    public struct TravelPoint
    {
        public Vector2 NormalisedPoint;
        public float ProportionCovered;
    }

    [SerializeField]
    private bool m_loop;

    [SerializeField]
    private bool m_destroyAtEnd;

    [SerializeField]
    private float m_travelTime;

    [SerializeField]
    private Vector2[] m_normalisedPoints;

    private TravelPoint[] m_travelPoints;

    private static readonly Vector3 k_invalidBounds = Vector3.zero;
    private static Vector3 m_bounds = k_invalidBounds;

    public bool Loop { get { return m_loop; } }
    public bool DestroyAtEnd { get { return m_destroyAtEnd; } }
    public float TravelTime { get { return m_travelTime; } }
    public Vector2[] NormalisedPoints { get { return m_normalisedPoints; } }
    public TravelPoint[] TravelPoints { get { return m_travelPoints; } }

    public static Vector3 Bounds
    {
        get
        {
            if (m_bounds == k_invalidBounds)
            {
                m_bounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0.0f));
            }
            return m_bounds;
        }
    }

    private void OnEnable()
    {
        Assert.IsTrue(m_normalisedPoints.Length > 1, $"Invalid path specified ({name}). All paths should have at least two points.");

        // Determine the sqr delta between all adjacent points
        float totalSqDelta= 0.0f;
        var deltas = new float[m_normalisedPoints.Length];
        for (int i = 1; i < m_normalisedPoints.Length; ++i)
        {
            deltas[i - 1] = (m_normalisedPoints[i] - m_normalisedPoints[i - 1]).magnitude;
            totalSqDelta +=  deltas[i - 1];
        }
        deltas[deltas.Length - 1] = (m_normalisedPoints[0] - m_normalisedPoints[m_normalisedPoints.Length - 1]).magnitude;
        
        if (m_loop)
        {
            totalSqDelta += deltas[deltas.Length - 1];
            m_travelPoints = new TravelPoint[m_normalisedPoints.Length + 1];
        }
        else
        {
            m_travelPoints = new TravelPoint[m_normalisedPoints.Length];
        }

        float previousProportion = 0.0f;
        m_travelPoints[0] = new TravelPoint { NormalisedPoint = m_normalisedPoints[0], ProportionCovered = 0.0f };
        for (int i = 1; i < m_normalisedPoints.Length; ++i)
        {
            var pointInfo = new TravelPoint { NormalisedPoint = m_normalisedPoints[i], ProportionCovered = (deltas[i - 1] / totalSqDelta) + previousProportion };
            previousProportion = pointInfo.ProportionCovered;
            m_travelPoints[i] = pointInfo;
        }

        if (m_travelPoints.Length > m_normalisedPoints.Length)
        {
            Assert.IsTrue(m_travelPoints.Length == m_normalisedPoints.Length + 1);
            m_travelPoints[m_travelPoints.Length - 1] = new TravelPoint { NormalisedPoint = m_normalisedPoints[0], ProportionCovered = 1.0f };
        }
    }


    #region IContextDefinition

    public PathContext MakeContext()
    {
        return new PathContext(this);
    }

    #endregion // IContextDefinition
}

public class PathContext : IContext
{
    public delegate void PathDelegate(PathContext a_pathContext);

    private bool m_destroyable;
    private PathDefinition m_definition;
    private float m_proportionTravelled;
    private Vector2 m_currNormalisedPosition;
    private Vector2 m_currPosition;

    public event PathDelegate OnDestroyable;

    public PathDefinition Definition { get { return m_definition; } }
    public Vector2 CurrentPosition { get { return m_currPosition; } }
    
    public PathContext(PathDefinition a_definition)
    {
        m_definition = a_definition;
        m_currNormalisedPosition = m_definition.NormalisedPoints[0];
    }


    #region IContext

    public void Process(float a_deltaTime)
    {
        // Determine the proportion travelled
        m_proportionTravelled += a_deltaTime / m_definition.TravelTime;
        if (m_proportionTravelled > 1.0f)
        {
            if (m_definition.Loop)
            {
                do
                {
                    m_proportionTravelled = m_proportionTravelled - 1.0f;
                } while (m_proportionTravelled > 1.0f);
            }
            else
            {
                m_proportionTravelled = Mathf.Min(m_proportionTravelled, 1.0f);
            }
        }

        // Determine the current destination point
        int destinationTravelPointIndex = -1;
        for (int i = 0; i < m_definition.TravelPoints.Length; ++i)
        {
            if (m_proportionTravelled <= m_definition.TravelPoints[i].ProportionCovered)
            {
                destinationTravelPointIndex = i;
                break;
            }
        }
        Assert.IsTrue(destinationTravelPointIndex >= 0);

        // Update the current position to match the proportion travelled
        var destinationTravelPoint = m_definition.TravelPoints[destinationTravelPointIndex];
        if (destinationTravelPointIndex == 0)
        {
            m_currNormalisedPosition = destinationTravelPoint.NormalisedPoint;
        }
        else
        {
            var previousDestinationTravelPoint = m_definition.TravelPoints[destinationTravelPointIndex - 1];
            float t = (destinationTravelPoint.ProportionCovered - m_proportionTravelled) / (destinationTravelPoint.ProportionCovered - previousDestinationTravelPoint.ProportionCovered);
            m_currNormalisedPosition = Vector2.Lerp(destinationTravelPoint.NormalisedPoint, previousDestinationTravelPoint.NormalisedPoint, t);
        }

        m_currPosition = Vector3.zero;
        m_currPosition.x = Mathf.LerpUnclamped(-PathDefinition.Bounds.x, PathDefinition.Bounds.x, m_currNormalisedPosition.x);
        m_currPosition.y = Mathf.LerpUnclamped(-PathDefinition.Bounds.y, PathDefinition.Bounds.y, m_currNormalisedPosition.y);

        // Mark the movable as destroyable
        if (m_definition.DestroyAtEnd
            && m_proportionTravelled >= 1.0f)
        {
            if (m_destroyable)
            {
                return;
            }
            m_destroyable = true;
            
            if (OnDestroyable != null)
            {
                OnDestroyable(this);
            }
        }
    }

    #endregion // IContext
}
