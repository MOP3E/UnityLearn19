using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Временная неуязвимость.
    /// </summary>
    public class PowerupGodMode : Powerup
    {
        /// <summary>
        /// Продолжительность неуязвимости.
        /// </summary>
        [SerializeField] private float _godTime;

        protected override void OnPickup(SpaceShip ship)
        {
            CollisionDamageApplicator applicator = ship.transform.root.GetComponent<CollisionDamageApplicator>();
            applicator.GodTime += _godTime;
        }
    }
}
