using UnityEngine;
using UnityEngine.Assertions;

public class DynamicAudioSource : MonoBehaviour, IPooledObject
{
    private AudioSource m_audioSource;
    private bool m_manageSound;
    private bool m_hasStartedPlaying;

    public AudioSource AudioSource { get { return m_audioSource; } }

    private void Awake()
    {
        m_audioSource = gameObject.AddComponent<AudioSource>();
        Assert.IsNotNull(m_audioSource);
    }

    private void LateUpdate()
    {
        if (!m_manageSound)
        {
            return;
        }

        if (!m_hasStartedPlaying
            && m_audioSource.isPlaying)
        {
            m_hasStartedPlaying = true;
        }
        
        if (m_hasStartedPlaying
            && !m_audioSource.isPlaying)
        {
            ObjectPoolManager.Instance.Relinquish(EffectManager.Instance.DynamicAudioSourcePrefab, gameObject);
        }
    }


    #region IPooledObject

    public void OnClaimed()
    {
        m_audioSource.Stop();
        m_audioSource.clip = null;
        m_manageSound = true;
        m_hasStartedPlaying = m_audioSource.isPlaying;
    }

    public void OnRelinquished()
    {
        m_manageSound = false;
        m_hasStartedPlaying = false;
        m_audioSource.Stop();
        m_audioSource.clip = null;
    }

    public void Relinquish()
    {
        ObjectPoolManager.Instance.Relinquish(EffectManager.Instance.DynamicAudioSourcePrefab, gameObject);
    }

    #endregion // IPooledObject
}
