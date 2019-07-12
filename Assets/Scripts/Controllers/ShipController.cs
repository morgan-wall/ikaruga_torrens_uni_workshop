using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Ship))]
public abstract class ShipController : MonoBehaviour
{
    private Ship m_ship;

    public Ship Ship { get { return m_ship; } }

    protected abstract int ProjectileLayer { get; }

    public void FirePrimaryWeapon()
    {
        if (!Ship.Context.PrimaryWeaponContext.CanShoot)
        {
            return;
        }

        var configName = Ship.Context.PrimaryWeaponContext.Definition.WeaponKindDefinition.ConfigName;
        int weaponConfigIndex = Ship.Model.GetWeaponConfigurationIndex(configName);
        Assert.IsTrue(weaponConfigIndex >= 0, $"Unable to find weapon config ({configName}) on model ({Ship.Model.name})");
        if (weaponConfigIndex >= 0)
        {
            var weaponConfig = Ship.Model.WeaponConfigurations[weaponConfigIndex];
            Ship.Context.PrimaryWeaponContext.Shoot(weaponConfig.Muzzles, ProjectileLayer);
        }
    }

    public void FireSecondaryWeapon()
    {
        if (!Ship.Context.SecondaryWeaponContext.CanShoot)
        {
            return;
        }

        var configName = Ship.Context.SecondaryWeaponContext.Definition.WeaponKindDefinition.ConfigName;
        int weaponConfigIndex = Ship.Model.GetWeaponConfigurationIndex(configName);
        Assert.IsTrue(weaponConfigIndex >= 0, $"Unable to find weapon config ({configName}) on model ({Ship.Model.name})");
        if (weaponConfigIndex >= 0)
        {
            var weaponConfig = Ship.Model.WeaponConfigurations[weaponConfigIndex];
            Ship.Context.SecondaryWeaponContext.Shoot(weaponConfig.Muzzles, ProjectileLayer);
        }
    }

    public void SetShieldElement(Element a_element)
    {
        Ship.Context.ShieldElement = a_element;
    }

    protected virtual void Awake()
    {
        m_ship = GetComponent<Ship>();
    }
}
