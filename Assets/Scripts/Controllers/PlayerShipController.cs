using System;
using UnityEngine;

public class PlayerShipController : ShipController
{
    private static readonly string k_projectileLayerName = "PlayerProjectiles";
    private static int k_projectileLayer = -1;

    protected override int ProjectileLayer
    {
        get
        {
            if (k_projectileLayer < 0)
            {
                k_projectileLayer = LayerMask.NameToLayer(k_projectileLayerName);
            }
            return k_projectileLayer;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void Update()
    {
        float horizontalAxisValue = Input.GetAxis("Horizontal");
        float xDelta = horizontalAxisValue * Ship.Context.Speed * Time.deltaTime;
        
        float verticalAxisValue = Input.GetAxis("Vertical");
        float yDelta = verticalAxisValue * Ship.Context.Speed * Time.deltaTime;
        
        transform.Translate(new Vector3(xDelta, yDelta, 0.0f));

        if (Input.GetButton("PrimaryAttack"))
        {
            FirePrimaryWeapon();
        }

        if (Input.GetButtonDown("ToggleShield"))
        {
            Element newElement = Element.None;
            switch (Ship.Context.ShieldElement)
            {
                case Element.None:
                    break;
                case Element.Red:
                    newElement = Element.Blue;
                    break;
                case Element.Blue:
                    newElement = Element.Red;
                    break;

                default:
                    throw new NotImplementedException();
            }

            SetShieldElement(newElement);
        }
    }
}