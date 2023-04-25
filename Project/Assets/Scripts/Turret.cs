using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Турель. Стреляет снарядами.
    /// </summary>
    public class Turret : MonoBehaviour
    {
        /// <summary>
        /// Тип турели.
        /// </summary>
        [SerializeField] private TurretType _type;

        /// <summary>
        /// Тип турели.
        /// </summary>
        public TurretType Type => _type;

        /// <summary>
        /// Свойства турели.
        /// </summary>
        [SerializeField] private TurretProperties _properties;

        /// <summary>
        /// Таймер перезарядки турели.
        /// </summary>
        private float _reloadTimer;

        /// <summary>
        /// Период стрельбы турели.
        /// </summary>
        private float _firePeriod;

        public bool CanFire => _reloadTimer <= 0;

        /// <summary>
        /// Космический корабль.
        /// </summary>
        private SpaceShip _ship;

        private Collider2D _shipCollider;

        /// <summary>
        /// Start запускается перед первым кадром.
        /// </summary>
        private void Start()
        {
            Transform root = transform.root;
            _ship = root.GetComponent<SpaceShip>();
            _shipCollider = root.GetComponentInChildren<Collider2D>();
        }

        /// <summary>
        /// Update запускается каждый кадр.
        /// </summary>
        private void Update()
        {
            if(_reloadTimer > 0) _reloadTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Стрельба из турели.
        /// </summary>
        public void Fire()
        {
            if(_properties == null || _reloadTimer > 0) return;

            //попытаться выстрелить
            switch (_type)
            {
                case TurretType.Primary:
                    if(!_ship.DrawEnergy(_properties.EnergyUsage)) return;
                    break;
                case TurretType.Secondary:
                    if (!_ship.DrawAmmo(_properties.AmmoUsage)) return;
                    break;
            }

            //создать снаряд
            Projectile projectile = Instantiate(_properties.ProjectilePrefab).GetComponent<Projectile>();

            ////запретить столкновения между снарядом и кораблём
            //Collider2D projectileCollider = projectile.transform.root.GetComponentInChildren<Collider2D>();
            //if(projectileCollider != null) Physics2D.IgnoreCollision(_shipCollider, projectileCollider);

            //задать позициюи направление движения снаряда
            Transform projectileTransform = projectile.transform;
            Transform myTransform = transform;
            projectileTransform.position = myTransform.position;
            projectileTransform.up = myTransform.up;

            //запретить столкновения между снарядом и кораблём
            projectile.ParentDestructible = _ship;

            //перезапустить таймер перезарядки
            _reloadTimer = 60 / _properties.FireRate;

            //todo воспроизвести звук выстрела (домашнее задание)
        }

        /// <summary>
        /// Загрузка других свойств турели.
        /// </summary>
        public void AssignLoadout(TurretProperties properties)
        {
            //проверка на соответствие типа полученных свойств с типом турели
            if(_type != properties.Type) return;

            _reloadTimer = 0;
            _properties = properties;
        }
    }
}
