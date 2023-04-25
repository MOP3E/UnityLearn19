using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Уничтожение объекта по истечении его времени жизни.
    /// </summary>
    public class ImpactEffect : MonoBehaviour
    {
        /// <summary>
        /// Продолжительность жизни объекта.
        /// </summary>
        [SerializeField] private float _lifetime;

        /// <summary>
        /// Счётчик времени жизни объекта.
        /// </summary>
        private float _lifeTimer = 0;

        /// <summary>
        /// Update запускается каждый кадр.
        /// </summary>
        private void Update()
        {
            _lifeTimer += Time.deltaTime;
            if (_lifeTimer > _lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
