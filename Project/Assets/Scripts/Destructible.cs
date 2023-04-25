using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooter
{
    /// <summary>
    /// Делегат события разрушения объекта.
    /// </summary>
    /// <param name="gameObject">Игровой объект, который разрушился.</param>
    public delegate void DestructionEvent(GameObject gameObject);

    /// <summary>
    /// Уничтожаемый объект, у которого есть некоторое количество очков жизни.
    /// </summary>
    public class Destructible : Entity
    {
        /// <summary>
        /// Это неразрушимый объект.
        /// </summary>
        [SerializeField] private bool _indestrictible = false;

        /// <summary>
        /// Это неразрушимый объект.
        /// </summary>
        public bool IsIndestrictible => _indestrictible;

        /// <summary>
        /// Максимальное (и начальное) число очков жизни разрушаемого объекта.
        /// </summary>
        [SerializeField] private int _maxHitpoints = 0;

        /// <summary>
        /// Очки жизни разрушаемого объекта.
        /// </summary>
        private int _hitpoints;

        /// <summary>
        /// Все разрушаемые объекты уровня.
        /// </summary>
        public static HashSet<Destructible> _allDestructibles;

        /// <summary>
        /// Все разрушаемые объекты уровня.
        /// </summary>
        public static IReadOnlyCollection<Destructible> AllDestructibles => _allDestructibles;

        /// <summary>
        /// Идентификатор нейтральной команды.
        /// </summary>
        public const int NeutralTeamId = 0;
        
        /// <summary>
        /// Очки жизни разрушаемого объекта.
        /// </summary>
        public int Hitpoints
        {
            get => _hitpoints;
            private set
            {
                _hitpoints = value;
                _hitpointsChange?.Invoke();
            }
        }

        /// <summary>
        /// Событие изменения очков жизни объекта.
        /// </summary>
        [SerializeField] private UnityEvent _hitpointsChange;

        /// <summary>
        /// Событие изменения очков жизни объекта.
        /// </summary>
        public UnityEvent HitpointsChange => HitpointsChange;

        /// <summary>
        /// Уже убит.
        /// </summary>
        private bool _isKilled = false;

        /// <summary>
        /// Событие юнити разрушения объекта.
        /// </summary>
        [SerializeField] private UnityEvent _destruction;

        /// <summary>
        /// Событие юнити разрушения объекта.
        /// </summary>
        public UnityEvent DestructionUnity => _destruction;

        /// <summary>
        /// Событие разрушения объекта.
        /// </summary>
        public DestructionEvent Destruction;

        /// <summary>
        /// Идентификатор команды этого корабля.
        /// </summary>
        [SerializeField] private int _teamId;

        /// <summary>
        /// Идентификатор команды этого корабля.
        /// </summary>
        public int TeamId
        {
            get => _teamId;
            set => _teamId = value;
        }

        /// <summary>
        /// Вызывается перед первым кадром.
        /// </summary>
        protected virtual void Start()
        {
            Hitpoints = _maxHitpoints;
        }

        /// <summary>
        /// Нанесение урона объекту.
        /// </summary>
        /// <param name="damage">Величина наносимого урона.</param>
        public void Hit(int damage)
        {
            if(_indestrictible) return;
            Hitpoints -= damage;
            if (Hitpoints <= 0) Kill();
        }

        /// <summary>
        /// Лечение объекта.
        /// </summary>
        /// <param name="cure">Величина лечения.</param>
        public bool Cure(int cure)
        {
            if (Hitpoints >= _maxHitpoints) return false;
            Hitpoints += cure;
            if (Hitpoints > _maxHitpoints) Hitpoints = _maxHitpoints;
            return true;
        }

        /// <summary>
        /// Гарантированное убийство объекта.
        /// </summary>
        protected virtual void Kill()
        {
            if (_isKilled) return;

            _isKilled = true;
            Hitpoints = 0;
            GameObject go = gameObject;
            _destruction?.Invoke();
            Destruction?.Invoke(go);
            _allDestructibles.Remove(this);
            Destroy(go);
        }

        /// <summary>
        /// Получить нормализованное число очков жизни.
        /// </summary>
        public float GetNormalizedHitpoints()
        {
            return Hitpoints < _maxHitpoints ? (float)Hitpoints / (float)_maxHitpoints : 1f;
        }

        private void OnEnable()
        {
            if (_allDestructibles == null) _allDestructibles = new HashSet<Destructible>();
            _allDestructibles.Add(this);
        }

        private void OnDestroy()
        {
            //_allDestructibles.Remove(this);
        }
    }
}
