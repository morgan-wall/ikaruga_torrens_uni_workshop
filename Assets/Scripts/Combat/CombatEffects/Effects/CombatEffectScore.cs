using UnityEngine;

public class CombatEffectScore : CombatEffect
{
    [SerializeField]
    private int m_amount = default;

    protected override bool Apply(ApplyContext a_applyContext)
    {
        var sessionManager = GameManager.Instance;
        if (sessionManager == null)
        {
            return false;
        }

        var levelContext = sessionManager.LevelContext;
        if (levelContext == null)
        {
            return false;
        }

        levelContext.Score += m_amount;
        return true;
    }
}
