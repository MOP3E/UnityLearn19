using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class CollisionDamageApplicator : MonoBehaviour
    {
        public static string IgnoreTag1 = "WorldBoundary";
        public static string IgnoreTag2 = "MyProjectile";

        /// <summary>
        /// Базовый урон.
        /// </summary>
        [SerializeField] private float _baseDamage;

        /// <summary>
        /// Модификатор урона от скорости.
        /// </summary>
        [SerializeField] private float _velocityDamageModifier;

        /// <summary>
        /// Режим неуязвимости.
        /// </summary>
        public float GodTime { get; set; }

        private CollisionDamageApplicator()
        {
            GodTime = 0;
        }

        private void Update()
        {
            if (GodTime > 0) 
                GodTime -= Time.deltaTime;
            else if(GodTime < 0) 
                GodTime = 0;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if(col.transform.root.CompareTag(IgnoreTag1)) return;
            if(col.transform.root.CompareTag(IgnoreTag2)) return;
            if(GodTime > 0) return;

            Destructible destructible = transform.root.GetComponent<Destructible>();
            if(destructible == null) return;

            destructible.Hit((int)(_baseDamage + _velocityDamageModifier * col.relativeVelocity.magnitude));
        }
    }
}
