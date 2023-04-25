using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class Powerup : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            SpaceShip ship = other.transform.root.GetComponent<SpaceShip>();
            if(ship == null || ship != Player.Instance.ActiveShip) return;

            Destroy(gameObject);
            OnPickup(ship);
        }

        protected abstract void OnPickup(SpaceShip ship);
    }
}
