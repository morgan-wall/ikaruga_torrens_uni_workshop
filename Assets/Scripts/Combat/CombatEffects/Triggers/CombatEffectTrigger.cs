using UnityEngine;
using UnityEngine.Assertions;

public abstract class CombatEffectTrigger : CombatEffect
{
    private bool m_registered;

    protected bool Registered { get { return m_registered; } }

    protected virtual void Register()
    {
        Assert.IsTrue(!m_registered);
        m_registered = true;
    }

    protected virtual void Deregister()
    {
        Assert.IsTrue(m_registered);
        m_registered= false;
    }

    protected override bool Apply(ApplyContext a_applyContext)
    {
        Register();
        return true;
    }

    protected override void Revert(RevertContext a_revertContext)
    {
        Deregister();
    }
}
