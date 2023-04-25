using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class PowerupVelocity : Powerup
    {
        /// <summary>
        /// Продолжительность повышения скорости.
        /// </summary>
        [SerializeField] private float _velocityTime;

        /// <summary>
        /// Сила повышения скорости.
        /// </summary>
        [SerializeField] private float _velocityPower;

        protected override void OnPickup(SpaceShip ship)
        {
            ship.VelocityPowerupTimer += _velocityTime;
            ship.VelocityPowerup = _velocityPower;
        }
    }
}
