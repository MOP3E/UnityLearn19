using UnityEngine;

namespace SpaceShooter
{
    public class Rotator : MonoBehaviour
    {
        /// <summary>
        /// Трансформа, которую нужно вращать.
        /// </summary>
        [SerializeField] private Transform _gameObject;

        /// <summary>
        /// Скорость вращения, единиц/с.
        /// </summary>
        [SerializeField] private Vector3 _speed;

        /// <summary>
        /// Перемещение объекта в заданную точку.
        /// </summary>
        private void Update()
        {
            _gameObject.Rotate(_speed * Time.deltaTime);
        }
    }
}