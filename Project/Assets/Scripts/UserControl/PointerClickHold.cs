using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SpaceShooter
{
    /// <summary>
    /// Сообщает, удерживается ли указатель на элементе интерфейса.
    /// </summary>
    public class PointerClickHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// На объекте нажата кнопка.
        /// </summary>
        private bool _isHold = false;

        /// <summary>
        /// На объекте нажата кнопка.
        /// </summary>
        public bool IsHold => _isHold;

        /// <summary>
        /// Start запускается перед первым кадром.
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// Update запускается каждый кадр.
        /// </summary>
        private void Update()
        {
        
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isHold = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHold = false;
        }
    }
}
