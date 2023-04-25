using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    /// <summary>
    /// Поднимание предмета игроком.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        /// <summary>
        /// Вещь поднята.
        /// </summary>
        public UnityEvent Pickuped;

        /// <summary>
        /// Уничтожение объекта при столкновении с танком.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null) return;
            if (collision.transform.root.GetComponent<SpaceShip>() == null) return;

            Pickuped?.Invoke();

            //удалить игровой объект из списка разрушаемых объектов
            //Globals.Instance.RemoveGameObject(gameObject);

            //самоуничтожиться
            Destroy(gameObject);
        }
    }
}