using UnityEngine;

public abstract class Effect : MonoBehaviour, IEffect
{
    [SerializeField]
    private string m_beginId = default;

    [SerializeField]
    private string m_endId = default;

    public string BeginId { get { return m_beginId; } }
    public string EndId { get { return m_beginId; } }


    #region IEffect

    public abstract void Begin(Model a_model);

    public abstract void End(Model a_model);

    #endregion // IEffect
}
