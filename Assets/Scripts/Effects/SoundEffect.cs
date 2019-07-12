using UnityEngine;
using UnityEngine.Assertions;

public class SoundEffect : Effect
{
    [SerializeField]
    private AudioClip m_clip;

    private static GameObject k_dynamicAudioSource;
    private static GameObject DynamicAudioSourcePrefab
    {
        get
        {
            if (k_dynamicAudioSource == null)
            {
                k_dynamicAudioSource = EffectManager.Instance != null ? EffectManager.Instance.DynamicAudioSourcePrefab : null;
                Assert.IsTrue(EffectManager.Instance == null || k_dynamicAudioSource != null);
            }
            return k_dynamicAudioSource;
        }
    }

    public override void Begin(Model a_model)
    {
        var dynamicAudioSource = ObjectPoolManager.Instance.Claim(DynamicAudioSourcePrefab).GetComponent<DynamicAudioSource>();
        Assert.IsNotNull(dynamicAudioSource);

        if (a_model != null)
        {
            dynamicAudioSource.transform.parent = a_model.DynamicEffectAttachment;
            dynamicAudioSource.transform.localPosition = Vector3.zero;
        }
        
        // N.B. We force the audio source to be active here to ensure it plays when the model is cleaned up.
        dynamicAudioSource.gameObject.SetActive(true);

        dynamicAudioSource.AudioSource.clip = m_clip;
        dynamicAudioSource.AudioSource.Play();
    }

    public override void End(Model a_model)
    {
    }
}
