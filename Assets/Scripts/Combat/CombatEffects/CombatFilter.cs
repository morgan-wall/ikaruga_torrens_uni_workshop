using UnityEngine;

public abstract class CombatFilter : MonoBehaviour
{
    public abstract bool Check(ApplyContext a_applyContext);
}
