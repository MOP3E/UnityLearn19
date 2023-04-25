using System.Collections;
using System.Collections.Generic;
using SpaceShooter.UserControl;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

namespace SpaceShooter
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceShip : Destructible
    {
        /// <summary>
        /// Скрипт управления кораблём.
        /// </summary>
        [Header("Space Ship")]
        [SerializeField] private ShipController _movementController;

        /// <summary>
        /// Скрипт управления кораблём.
        /// </summary>
        public ShipController Controller
        {
            get => _movementController;
            set => _movementController = value;
        }

        /// <summary>
        /// Масса.
        /// </summary>
        [SerializeField] private float _mass;

        /// <summary>
        /// Полное ускорение прямолинейного движения.
        /// </summary>
        [SerializeField] private float _acceleration;

        /// <summary>
        /// Полное ускорение поворота.
        /// </summary>
        [SerializeField] private float _angularAcceleration;

        /// <summary>
        /// Максимальная линейная скорость.
        /// </summary>
        [SerializeField] private float _maxVelocity;

        /// <summary>
        /// Время действия бонуса скорости.
        /// </summary>
        public float VelocityPowerupTimer { get; set; }

        /// <summary>
        /// Сила бонуса скорости.
        /// </summary>
        public float VelocityPowerup { get; set; }

        /// <summary>
        /// Максимальная скорость поворота, градус/с.
        /// </summary>
        [SerializeField] private float _maxAngularVelocity;

        /// <summary>
        /// Ссылка на физическое тело корабля.
        /// </summary>
        private Rigidbody2D _myRigidbody;
        
        /// <summary>
        /// Турели космического корабля.
        /// </summary>
        [SerializeField] private Turret[] _turrets;

        /// <summary>
        /// Максимальная энергия космического корабля.
        /// </summary>
        [SerializeField] private int _maxEnergy;

        /// <summary>
        /// Начальная энергия космического корабля. Если не используется - поставить значение меньше 0.
        /// </summary>
        [SerializeField] private int _startEnergy;

        /// <summary>
        /// Восстановление энергии корабля.
        /// </summary>
        [SerializeField] private int _energyRegenPerSecond;

        /// <summary>
        /// Максимальный боезапас космического корабля.
        /// </summary>
        [SerializeField] private int _maxAmmo;

        /// <summary>
        /// Начальный боезапас космического корабля. Если не используется - поставить значение меньше 0.
        /// </summary>
        [SerializeField] private int _startAmmo;

        /// <summary>
        /// Энергия основного оружия космического корабля.
        /// </summary>
        private float _primaryEnergy;

        /// <summary>
        /// Боеприпасы вторичного оружия космического корабля.
        /// </summary>
        private int _secondaryAmmo;
        
        /// <summary>
        /// Start запускается перед первым кадром.
        /// </summary>
        protected override void Start()
        {
            _myRigidbody = GetComponent<Rigidbody2D>();
            _myRigidbody.mass = _mass;
            _myRigidbody.inertia = 1;

            _primaryEnergy = _startEnergy >= 0 ? _startEnergy : _maxEnergy;
            _secondaryAmmo = _startAmmo >=0 ? _startAmmo : _maxAmmo;

            base.Start();
        }

        /// <summary>
        /// Update запускается каждый кадр.
        /// </summary>
        private void Update()
        {
            //обработка стрельбы из основного либо вторичного оружия
            if (_movementController != null)
            {
                //стрельба из основного оружия
                if (_movementController.PrimaryFireButton > 0f) Fire(TurretType.Primary);
                //стрельба из вторичного оружия
                if (_movementController.SecondaryFireButton > 0f) Fire(TurretType.Secondary);
            }

            if (VelocityPowerupTimer > 0)
                VelocityPowerupTimer -= Time.deltaTime;
            else if (VelocityPowerupTimer < 0)
                VelocityPowerupTimer = 0;
        }

        /// <summary>
        /// FixedUpdate запускается с фиксированным перидом.
        /// </summary>
        private void FixedUpdate()
        {
            //обработка физики движения корабля
            RigidBodyUpdate();
            //восстановление энергии корабля
            EnergyRegen();
        }

        /// <summary>
        /// ОБработка физики движения корабля.
        /// </summary>
        private void RigidBodyUpdate()
        {
            if(_movementController == null) return;

            if (VelocityPowerupTimer > 0)
            {
                //бонус удвоения скорости
                //линейное движение
                //примененине команды от игрока
                _myRigidbody.AddForce(_acceleration * VelocityPowerup * _movementController.AccelerationAxis * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);
                //торможение шершавым космическим вакуумом
                _myRigidbody.AddForce(-_myRigidbody.velocity * (_acceleration / _maxVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

                //поворот
                //примененине команды от игрока
                _myRigidbody.AddTorque(_angularAcceleration * VelocityPowerup * _movementController.AngularAccelerationAxis * Time.fixedDeltaTime, ForceMode2D.Force);
                //торможение шершавым космическим вакуумом
                _myRigidbody.AddTorque(-_myRigidbody.angularVelocity * (_angularAcceleration / _maxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);
            }
            else
            {
                //стандартная скорость
                //линейное движение
                //примененине команды от игрока
                _myRigidbody.AddForce(_acceleration * _movementController.AccelerationAxis * transform.up * Time.fixedDeltaTime, ForceMode2D.Force);
                //торможение шершавым космическим вакуумом
                _myRigidbody.AddForce(-_myRigidbody.velocity * (_acceleration / _maxVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

                //поворот
                //примененине команды от игрока
                _myRigidbody.AddTorque(_angularAcceleration * _movementController.AngularAccelerationAxis * Time.fixedDeltaTime, ForceMode2D.Force);
                //торможение шершавым космическим вакуумом
                _myRigidbody.AddTorque(-_myRigidbody.angularVelocity * (_angularAcceleration / _maxAngularVelocity) * Time.fixedDeltaTime, ForceMode2D.Force);

            }
        }

        /// <summary>
        /// Стрельба из турелей космического корабля.
        /// </summary>
        public void Fire(TurretType turretType)
        {
            foreach (Turret turret in _turrets)
            {
                if (turret.Type != turretType) continue;
                turret.Fire();
            }
        }

        /// <summary>
        /// Добавление энергии кораблю.
        /// </summary>
        public void AddEnergy(int energy)
        {
            _primaryEnergy += energy;
            if (_primaryEnergy > _maxEnergy) _primaryEnergy = _maxEnergy;
        }

        /// <summary>
        /// Добавление патронов кораблю.
        /// </summary>
        public void AddAmmo(int ammo)
        {
            _secondaryAmmo += ammo;
            if (_secondaryAmmo > _maxAmmo) _secondaryAmmo = _maxAmmo;
        }

        /// <summary>
        /// Регенрация энергии.
        /// </summary>
        private void EnergyRegen()
        {
            _primaryEnergy += (float)_energyRegenPerSecond * Time.fixedDeltaTime;
            if (_primaryEnergy > _maxEnergy) _primaryEnergy = _maxEnergy;
        }

        /// <summary>
        /// Расходование патронов.
        /// </summary>
        public bool DrawAmmo(int count)
        {
            if (count == 0) return true;

            if (_secondaryAmmo < count) return false;

            _secondaryAmmo -= count;
            return true;
        }

        /// <summary>
        /// Расходование энергии.
        /// </summary>
        public bool DrawEnergy(int count)
        {
            if (count == 0) return true;

            if (_primaryEnergy < count) return false;

            _primaryEnergy -= count;
            return true;
        }

        /// <summary>
        /// Назначить новые свойства турелям корабля.
        /// </summary>
        /// <param name="properties">Свойства, которые  нужно назначить.</param>
        public void AssignWeapon(TurretProperties properties)
        {
            foreach (Turret turret in _turrets)
            {
                turret.AssignLoadout(properties);
            }
        }
    }
}
