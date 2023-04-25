using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class PowerupWeapon : Powerup
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private TurretProperties _turretProperties;

        protected override void OnPickup(SpaceShip ship)
        {
            ship.AssignWeapon(_turretProperties);
        }
    }
}
