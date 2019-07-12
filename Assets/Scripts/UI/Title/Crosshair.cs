using System;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Serializable]
    private struct MoveInfo
    {
        public float Duration;
        public float DegreesPerSecond;
    }

    [SerializeField]
    private MoveInfo[] m_moveSequence;

    private int m_nextMoveInfoIndex;
    private float m_timeElapsedForCurrentMove;

    private void Update()
    {
        float timeRemaining = Time.deltaTime;
        while (timeRemaining > 0.0f)
        {
            // Determine the amount of time actioned by the current move
            var currMoveInfo = m_moveSequence[m_nextMoveInfoIndex];
            float maxTimeActionable = Mathf.Max(currMoveInfo.Duration - m_timeElapsedForCurrentMove, 0.0f);
            float timeActioned = Mathf.Min(maxTimeActionable, timeRemaining);

            // Update time tracking
            timeRemaining -= timeActioned;
            m_timeElapsedForCurrentMove += timeActioned;

            // Perform the move
            transform.Rotate(Vector3.forward, currMoveInfo.DegreesPerSecond * timeActioned);

            // Transition to the next move, if possible
            if (currMoveInfo.Duration <= m_timeElapsedForCurrentMove)
            {
                m_timeElapsedForCurrentMove = 0.0f;
                m_nextMoveInfoIndex = (m_nextMoveInfoIndex + 1) % m_moveSequence.Length;
            }
        }
    }
}
