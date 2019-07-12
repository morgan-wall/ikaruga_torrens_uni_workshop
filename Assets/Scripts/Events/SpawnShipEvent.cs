using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(menuName = "Game/Events/Spawn Ship")]
public class SpawnShipEvent : Event
{
    [SerializeField]
    private ShipDefinition m_shipDefinition;

    [SerializeField]
    private PathDefinition m_pathDefinition;

    [SerializeField]
    private BehaviourDefinition m_behaviourDefinition;

    public override void Execute(GameManager a_sessionManager)
    {
        var ship = m_shipDefinition.MakeObject();
        var enemyShipController = ship.GetComponent<EnemyShipController>();
        Assert.IsNotNull(enemyShipController);
        enemyShipController.Bind(m_pathDefinition.MakeContext());
        enemyShipController.Bind(m_behaviourDefinition.MakeContext());
        enemyShipController.Init();
    }
}
