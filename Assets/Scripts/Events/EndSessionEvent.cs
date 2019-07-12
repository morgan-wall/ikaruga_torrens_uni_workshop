using UnityEngine;

[CreateAssetMenu(menuName = "Game/Events/End Session")]
public class EndSessionEvent : Event
{
    public override void Execute(GameManager a_sessionManager)
    {
        a_sessionManager.End();
    }
}
