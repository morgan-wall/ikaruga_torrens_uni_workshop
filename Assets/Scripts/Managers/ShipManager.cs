using System.Collections.Generic;
using UnityEngine.Assertions;

public class ShipManager
{
    private List<Ship> m_ships = new List<Ship>();

    private static ShipManager s_instance;
    public static ShipManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new ShipManager();
            }
            return s_instance;
        }
    }

    public void Add(Ship a_ship)
    {
        Assert.IsTrue(!m_ships.Contains(a_ship));
        m_ships.Add(a_ship);
    }

    public void Remove(Ship a_ship)
    {
        Assert.IsTrue(m_ships.Contains(a_ship));
        m_ships.Remove(a_ship);
    }

    public void RemoveAll()
    {
        for (int i = m_ships.Count - 1; i >= 0; --i)
        {
            var ship = m_ships[i];
            ObjectPoolManager.Instance.Relinquish(ship.Context.Definition.Prefab.gameObject, ship.gameObject);
        }
    }
}
