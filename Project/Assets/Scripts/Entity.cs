using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Базовый класс всех интерактивных игровых объектов на сцене.
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        /// <summary>
        /// Название объекта пользователя.
        /// </summary>
        [SerializeField] private string _nickname;

        /// <summary>
        /// Название объекта пользователя.
        /// </summary>
        public string Nickname => _nickname;
        
        public override string ToString() => _nickname;
    }
}
