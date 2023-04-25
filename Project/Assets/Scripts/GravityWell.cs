using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class GravityWell : MonoBehaviour
    {
        /// <summary>
        /// Сила  гравитационного колодца.
        /// </summary>
        [SerializeField] private float _force;

        /// <summary>
        /// Радиус гравитационного колодца.
        /// </summary>
        [SerializeField] private float _radius;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.attachedRigidbody == null) return;

            Vector2 direction = transform.position - other.transform.position;
            float distance = direction.magnitude;

            if (distance < _radius)
            {
                //применение притяжения к объекту
                Vector2 force = direction.normalized * _force * (distance / _radius);
                other.attachedRigidbody.AddForce(force, ForceMode2D.Force);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Валидация значения, изменённого в редакторе. Функция редактора.
        /// </summary>
        private void OnValidate()
        {
            //сделать радиус коллайдера гравитационного колодца таким же, как радиус силы притяежния
            GetComponent<CircleCollider2D>().radius = _radius;
        }
#endif
    }
}
