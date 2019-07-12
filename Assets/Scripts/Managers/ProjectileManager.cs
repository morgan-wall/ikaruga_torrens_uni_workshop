using System.Collections.Generic;
using UnityEngine.Assertions;

public class ProjectileManager
{
    private List<Projectile> m_projectiles = new List<Projectile>();

    private static ProjectileManager s_instance;
    public static ProjectileManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new ProjectileManager();
            }
            return s_instance;
        }
    }

    public void Add(Projectile a_projectile)
    {
        Assert.IsTrue(!m_projectiles.Contains(a_projectile));
        m_projectiles.Add(a_projectile);
    }

    public void Remove(Projectile a_projectile)
    {
        Assert.IsTrue(m_projectiles.Contains(a_projectile));
        m_projectiles.Remove(a_projectile);
    }

    public void RemoveAll()
    {
        for (int i = m_projectiles.Count - 1; i >= 0; --i)
        {
            var projectile = m_projectiles[i];
            ObjectPoolManager.Instance.Relinquish(projectile.Context.Definition.Prefab.gameObject, projectile.gameObject);
        }
    }
}
