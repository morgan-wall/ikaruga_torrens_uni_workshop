using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [SerializeField]
    private float m_maxOffset = default;

    [SerializeField]
    private float m_degreesPerSecond = default;

    private bool m_increasing;
    private float m_currOffset;
    private Quaternion m_defaultOrientation;

    private void Start()
    {
        m_defaultOrientation = transform.rotation;
    }

    private void Update()
    {
        float degreesRemaining = m_degreesPerSecond * Time.deltaTime;
        while (degreesRemaining > 0.0f)
        {
            // Determine the actionable degrees
            float maxDegreesAvailable = m_maxOffset - m_currOffset;
            if (!m_increasing)
            {
                maxDegreesAvailable = Mathf.Abs(-m_maxOffset - m_currOffset);
            }
            float degreesActioned = Mathf.Min(maxDegreesAvailable, degreesRemaining);

            // Update degree tracking
            degreesRemaining -= degreesActioned;
            if (m_increasing)
            {
                m_currOffset += degreesActioned;
                if (m_currOffset >= m_maxOffset)
                {
                    m_increasing = !m_increasing;
                }
            }
            else
            {
                m_currOffset -= degreesActioned;
                if (m_currOffset <= -m_maxOffset)
                {
                    m_increasing = !m_increasing;
                }
            }
        }

        // Apply the rotation
        transform.rotation = m_defaultOrientation * Quaternion.AngleAxis(m_currOffset, Vector3.forward);
    }
}
