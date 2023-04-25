using UnityEngine;

namespace SpaceShooter.UserControl
{
    /// <summary>
    /// Ось, управляемая одной кнопкой с клавиатуры.
    /// </summary>
    public class KeyboarButton : ControlAxis
    {
        /// <summary>
        /// Кнопка для "Полный вперёд".
        /// </summary>
        [SerializeField] private KeyCode _key;

        /// <summary>
        /// Update запускается каждый кадр.
        /// </summary>
        private void Update()
        {
            //1 - кнопка нажата; 0 - кнопка не нажата
            _value = Input.GetKey(_key) ? 1f : 0f;
        }
    }
}
