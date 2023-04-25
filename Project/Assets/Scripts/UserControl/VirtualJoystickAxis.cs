using System.Collections;
using UnityEngine;

namespace SpaceShooter.UserControl
{
    /// <summary>
    /// Ось виртуального джойстика.
    /// </summary>
    public class VirtualJoystickAxis : ControlAxis
    {
        /// <summary>
        /// Виртуальный джойстик, ось которого нужно обрабатывать.
        /// </summary>
        [SerializeField]private VirtualJoystick _joystick;

        /// <summary>
        /// Имя оси, которую нужно взять у джойстика.
        /// </summary>
        [SerializeField] private AxisNames _joystickAxisName;

        /// <summary>
        /// Эту ось нужно инвертировать.
        /// </summary>
        [SerializeField] private bool _inverse;

        /// <summary>
        /// Запускается каждый кадр.
        /// </summary>
        private void Update()
        {
            //копирование значения заданной оси джойстика в эту ось
            switch (_joystickAxisName)
            {
                case AxisNames.AxisX:
                    _value = _inverse ? -_joystick.Value.x : _joystick.Value.x;
                    break;
                case AxisNames.AxisY:
                    _value = _inverse ? -_joystick.Value.y : _joystick.Value.y;
                    break;
                case AxisNames.AxisZ:
                    _value = _inverse ? -_joystick.Value.z : _joystick.Value.z;
                    break;
                default:
                    _value = _inverse ? -_joystick.Value.x : _joystick.Value.x;
                    break;
            }
        }
    }
}
