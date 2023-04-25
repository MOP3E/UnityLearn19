using SpaceShooter.UserControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Контроллер космического корабля.
    /// </summary>
    public class ShipController : MonoBehaviour
    {
        /// <summary>
        /// Ось линейного ускорения.
        /// </summary>
        public virtual ControlAxis AccelerationAxis { get; set; }

        /// <summary>
        /// Кнопка стрельбы из основного оружия.
        /// </summary>
        public virtual ControlAxis AngularAccelerationAxis { get; set; }

        /// <summary>
        /// Кнопка стрельбы из основного оружия.
        /// </summary>
        public virtual ControlAxis PrimaryFireButton { get; set; }

        /// <summary>
        /// Кнопка стрельбы из вторичного оружия.
        /// </summary>
        public virtual ControlAxis SecondaryFireButton { get; set; }
    }
}
