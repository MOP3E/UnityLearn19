using SpaceShooter.UserControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class VirtualButton : ControlAxis
    {
        /// <summary>
        /// Виртуальная кнопка.
        /// </summary>
        [SerializeField] private PointerClickHold _virtualButton;

        /// <summary>
        /// Update запускается каждый кадр.
        /// </summary>
        private void Update()
        {
            //1 - кнопка нажата; 0 - кнопка не нажата
            _value = _virtualButton.IsHold ? 1 : 0;
        }
    }
}
