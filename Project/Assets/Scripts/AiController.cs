using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SpaceShooter
{
    public enum AiBehaviour
    {
        /// <summary>
        /// Ничего не делать.
        /// </summary>
        None,

        /// <summary>
        /// Патрулировать в пределах заданной зоны.
        /// </summary>
        ZonePatrol,

        /// <summary>
        /// Патрулировать по заданному маршруту, вперёд по списку точек.
        /// </summary>
        RoutePatrolOnward,

        /// <summary>
        /// Патрулировать по заданному маршруту, назад по списку точек.
        /// </summary>
        RoutePatrolBackward,

        /// <summary>
        /// Патрулировать по заданному маршруту, вперёд по списку точек, не отвлекаться на цели и не стрелять.
        /// </summary>
        DummyRoutePatrolOnward,
    }

    [RequireComponent(typeof(SpaceShip))]
    public class AiController : ShipController
    {
        /// <summary>
        /// Тип поведения ИИ.
        /// </summary>
        [SerializeField] private AiBehaviour _behaviour;

        /// <summary>
        /// Зона патрулирования корабля.
        /// </summary>
        [SerializeField] private AiPatrolZone _patrolZone;

        /// <summary>
        /// Массив точек маршрута патрулирования.
        /// </summary>
        [SerializeField] private Transform[] _patrolRoute;

        /// <summary>
        /// Точность выхода на точку патрулирования после чего корабль нацелится на следующую точку.
        /// </summary>
        [SerializeField] private float _patrolRoutePrecision;

        /// <summary>
        /// Коэффициент линейной скорости.
        /// </summary>
        [Range(0f, 1f)][SerializeField] private float _navigationLinear;

        private const float MaxAngle = 45f;

        /// <summary>
        /// Коэффициент угловой скорости.
        /// </summary>
        [Range(0f, 1f)][SerializeField] private float _navigationAngular;

        /// <summary>
        /// Время между выбором навигационных точек.
        /// </summary>
        [SerializeField] private float _randomSelectMovePointTime;

        /// <summary>
        /// Время между поиском новых целей.
        /// </summary>
        [SerializeField] private float _findNewTargetTime;

        /// <summary>
        /// Время между выстрелами.
        /// </summary>
        [SerializeField] private float _shootDelay;

        /// <summary>
        /// Длина рэйкаста.
        /// </summary>
        [SerializeField] private float _evadeRayLength;

        /// <summary>
        /// Корабль, которым управляет ИИ.
        /// </summary>
        private SpaceShip _spaceShip;

        /// <summary>
        /// Физическое тело корабля, которым управляет ИИ.
        /// </summary>
        private Rigidbody2D _spaceShipRigidbody2D;

        /// <summary>
        /// Позиция, в которую движется корабль.
        /// </summary>
        private Vector3 _movePosition;

        /// <summary>
        /// Противник, который выбран в качестве цели.
        /// </summary>
        private Destructible _selectedTarget;

        /// <summary>
        /// Физическое тело противника, который выбран в качествев цели.
        /// </summary>
        private Rigidbody2D _selectedTargetRigidbody2D;

        /// <summary>
        /// Таймер задержки выбора новой точки патрулирования при патрулировании в пределах зоны.
        /// </summary>
        private Timer _patrolZoneNewPointTimer;

        /// <summary>
        /// Таймер задержки стрельбы.
        /// </summary>
        private Timer _shootTimer;

        /// <summary>
        /// Таймер поиска новой цели.
        /// </summary>
        private Timer _findNewTargetTimer;

        /// <summary>
        /// Счётчик точек маршрута патрулирования.
        /// </summary>
        private int _patrolRouteCounter;

        //кэширование осей контроллера
        private AiAxis _accelerationAxis;
        private AiAxis _angularAccelerationAxis;
        private AiAxis _primaryFireButton;
        private AiAxis _secondaryFireButton;

        private void Start()
        {
            //создать оси контпроллера
            AccelerationAxis = gameObject.AddComponent<AiAxis>();
            _accelerationAxis = (AiAxis)AccelerationAxis;
            AngularAccelerationAxis = gameObject.AddComponent<AiAxis>();
            _angularAccelerationAxis = (AiAxis)AngularAccelerationAxis;
            PrimaryFireButton = gameObject.AddComponent<AiAxis>();
            _primaryFireButton = (AiAxis)PrimaryFireButton;
            SecondaryFireButton = gameObject.AddComponent<AiAxis>();
            _secondaryFireButton = (AiAxis)SecondaryFireButton;

            //закэшировать компоненты
            _spaceShip = GetComponent<SpaceShip>();
            _spaceShipRigidbody2D = GetComponent<Rigidbody2D>();

            //задать себя контроллером космическому кораблю
            _spaceShip.Controller = this;

            //настройка патрулирования по точкам
            if (_behaviour == AiBehaviour.RoutePatrolOnward || _behaviour == AiBehaviour.RoutePatrolBackward || _behaviour == AiBehaviour.DummyRoutePatrolOnward)
            {
                _patrolRouteCounter = 0;
                if (_patrolRoute != null && _patrolRoute.Length > 0) _movePosition = _patrolRoute[0].position;
            }

            InitTimers();
        }


        private void Update()
        {
            UpdateTimers();
            UpdateAi();
        }

        private void InitTimers()
        {
            _patrolZoneNewPointTimer = new Timer(_randomSelectMovePointTime);
            _shootTimer = new Timer(_shootDelay);
            _findNewTargetTimer = new Timer(_findNewTargetTime);
        }

        private void UpdateTimers()
        {
            //таймер патрулирования зоны тикает только при патрулировании зоны
            if (_behaviour == AiBehaviour.ZonePatrol) _patrolZoneNewPointTimer.SubstractTime(Time.deltaTime);
            _shootTimer.SubstractTime(Time.deltaTime);
            _findNewTargetTimer.SubstractTime(Time.deltaTime);
        }

        private void UpdateAi()
        {
            switch (_behaviour)
            {
                case AiBehaviour.None:
                    UpdatePatrolBehaviour();
                    break;
                case AiBehaviour.ZonePatrol:
                case AiBehaviour.RoutePatrolOnward:
                case AiBehaviour.RoutePatrolBackward:
                case AiBehaviour.DummyRoutePatrolOnward:
                    UpdatePatrolBehaviour();
                    break;
            }
        }

        private void UpdatePatrolBehaviour()
        {
            ActionFindNewMovePosition();
            if(_behaviour != AiBehaviour.DummyRoutePatrolOnward)
            {
                ActionEvadeCollision();
                ActionFoundNewTarget();
            }            
            ActionShipControl();
            if (_behaviour != AiBehaviour.DummyRoutePatrolOnward)
            {
                ActionFire();
            }
        }

        private void ActionFindNewMovePosition()
        {
            //если у корабля есть цель, он к ней движется и новую точку не ищет
            if (_selectedTarget != null)
            {
                //_movePosition = _selectedTarget.transform.position;
                _movePosition = MakeLead();
                return;
            }

            //изменитьть точку назначения согласно алгоритму поведения
            switch (_behaviour)
            {
                case AiBehaviour.ZonePatrol:
                    //патрулирование в пределах заданной зоны
                    if (_patrolZone == null) return;

                    //проверить, находится ли корабль внтури зоны патрулирования
                    bool isInsidePatrolZone = (_patrolZone.transform.position - transform.position).sqrMagnitude < _patrolZone.Radius * _patrolZone.Radius;

                    if (isInsidePatrolZone)
                    {
                        //корабль внутри зоны патрулиорования - выбрать случайную позицию в пределах зоны патрулирования
                        if (_patrolZoneNewPointTimer.IsDone)
                        {
                            Vector2 newPoint = UnityEngine.Random.onUnitSphere * _patrolZone.Radius + _patrolZone.transform.position;
                            _movePosition = newPoint;
                            _patrolZoneNewPointTimer.Start(_randomSelectMovePointTime);
                        }
                    }
                    else
                    {
                        //корабль за пределами зоны патрулирования - взять курс на центр зоны патрулированиня
                        _movePosition = _patrolZone.transform.position;
                    }
                    return;
                case AiBehaviour.RoutePatrolOnward:
                case AiBehaviour.RoutePatrolBackward:
                case AiBehaviour.DummyRoutePatrolOnward:
                    //патрулирование по заданному маршруту, заданному списком точек патрулирования
                    if (_patrolRoute == null || _patrolRoute.Length == 0) return;

                    //рассчитать дистанцию до заданной точки патрулирования
                    float distance = Vector2.Distance(_patrolRoute[_patrolRouteCounter].position, _spaceShip.transform.position);
                    //если корабль далеко от заданной точки патрулирования - закончить работу
                    if (distance > _patrolRoutePrecision) return;

                    //перейти к следующей точке патрулирования
                    if (_behaviour == AiBehaviour.RoutePatrolOnward || _behaviour == AiBehaviour.DummyRoutePatrolOnward)
                    {
                        _patrolRouteCounter++;
                        if (_patrolRouteCounter > _patrolRoute.Length - 1) _patrolRouteCounter = 0;
                    }
                    else
                    {
                        _patrolRouteCounter--;
                        if (_patrolRouteCounter < 0) _patrolRouteCounter = 0;
                    }

                    _movePosition = _patrolRoute[_patrolRouteCounter].position;
                    return;
            }
        }

        /// <summary>
        /// Уклонение от препятствий.
        /// </summary>
        private void ActionEvadeCollision()
        {
            if (Physics2D.Raycast(transform.position, transform.up, _evadeRayLength))
            {
                Transform transform1 = transform;
                _movePosition = transform1.position + transform1.right * 100f;
            }
        }


        private void ActionShipControl()
        {
            //применение линейного ускорения
            _accelerationAxis.AiValue = _navigationLinear;
            //применение углового ускорения
            _angularAccelerationAxis.AiValue = ComputeAlignTorqueNormalized(_movePosition, _spaceShip.transform) * _navigationAngular;
        }

        private static float ComputeAlignTorqueNormalized(Vector3 targetPosition, Transform ship)
        {
            Vector2 localTargetPosition = ship.InverseTransformPoint(targetPosition);
            float angle = Vector3.SignedAngle(localTargetPosition, Vector3.up, Vector3.forward);
            angle = Mathf.Clamp(angle, -MaxAngle, MaxAngle) / MaxAngle;
            return -angle;
        }

        /// <summary>
        /// Поиск новой цели.
        /// </summary>
        private void ActionFoundNewTarget()
        {
            //если цель уже выбрана либо таймер поиска цели не закончился - ничего не делать
            if (_selectedTarget != null || !_findNewTargetTimer.IsDone) return;

            //выбрать новую цель
            _selectedTarget = FindNearestDestructibleTarget();
            _selectedTargetRigidbody2D = _selectedTarget == null ? null : _selectedTarget.GetComponent<Rigidbody2D>();

            //перезапустить таймер поиска цели
            _findNewTargetTimer.Start(_findNewTargetTime);
        }

        /// <summary>
        /// Поиск ближайшей к кораблю цели.
        /// </summary>
        private Destructible FindNearestDestructibleTarget()
        {
            //текущая дистанция до цели
            float currentDistance = float.MaxValue;
            //текущая выбранная цель
            Destructible currentTarget = null;
            foreach (Destructible destructible in Destructible.AllDestructibles)
            {
                //пропускать свой собственный корабль, нейтральные и дружественные цели
                if (destructible.GetComponent<SpaceShip>() == _spaceShip ||
                   destructible.TeamId == Destructible.NeutralTeamId ||
                   destructible.TeamId == _spaceShip.TeamId) continue;

                //проверить дистанцию до цели
                float distance = Vector2.Distance(_spaceShip.transform.position, destructible.transform.position);
                if (distance > currentDistance) continue;
                
                //сохранить более близкую цель как текущую
                currentDistance = distance;
                currentTarget = destructible;
                //currentTarget.Destruction += OnSelectedTargetDestruction;
                currentTarget.DestructionUnity.AddListener(Call);
            }

            //вернуть более близкую цель
            return currentTarget;
        }

        private void Call()
        {
            _selectedTarget = null;
        }

        ///// <summary>
        ///// Событие уничтожения выбранной цели.
        ///// </summary>
        //private void OnSelectedTargetDestruction(GameObject gameobject)
        //{
        //    //_selectedTarget.Destruction -= OnSelectedTargetDestruction;
        //    _selectedTarget = null;
        //}

        private void ActionFire()
        {
            if (_selectedTarget == null || !_shootTimer.IsDone) return;
            
            _spaceShip.Fire(TurretType.Primary);
            _shootTimer.Start(_shootDelay);
        }

        /// <summary>
        /// Задать зону патрулирования.
        /// </summary>
        public void SetPatrolZone(AiPatrolZone zone)
        {
            _patrolZone = zone;
            _behaviour = _patrolZone == null ? AiBehaviour.None : AiBehaviour.ZonePatrol;
        }

        /// <summary>
        /// Задать маршрут патрулирования.
        /// </summary>
        public void SetPatrolRoute(Transform[] route, bool onward = true)
        {
            if (route != null && route.Length > 0)
            {
                _patrolRoute = new Transform[route.Length];
                for (int i = 0; i < route.Length; i++)
                {
                    _patrolRoute[i] = route[i];
                }
                _patrolRouteCounter = 0;
                _movePosition = _patrolRoute[0].position;
                _behaviour = onward ? AiBehaviour.RoutePatrolOnward : AiBehaviour.RoutePatrolBackward;
                return;
            }

            _behaviour = AiBehaviour.None;
            _patrolRoute = Array.Empty<Transform>();
        }

        /// <summary>
        /// Получить точку перехвата цели.
        /// </summary>
        private Vector2 MakeLead()
        {
            Vector2 shooterPosition = _spaceShip.transform.position;
            Vector2 shooterVelocity = _spaceShipRigidbody2D.velocity;
            float shotSpeed = shooterVelocity.magnitude;
            Vector2 targetPosition = _selectedTarget.transform.position;
            Vector2 targetVelocity = _selectedTargetRigidbody2D.velocity;

            return FirstOrderIntercept(shooterPosition, shooterVelocity, shotSpeed, targetPosition, targetVelocity);
        }

        /// <summary>
        /// Перехват первого порядка с использованием абсолютного положения цели
        /// </summary>
        /// <param name="shooterPosition">Позиция стрелка.</param>
        /// <param name="shooterVelocity">Скорость стрелка.</param>
        /// <param name="shotSpeed">Скорость выстрела.</param>
        /// <param name="targetPosition">Позиция цели.</param>
        /// <param name="targetVelocity">Скорость цели.</param>
        /// <returns></returns>
        private static Vector2 FirstOrderIntercept(Vector2 shooterPosition, Vector2 shooterVelocity, float shotSpeed, Vector2 targetPosition, Vector2 targetVelocity)
        {
            //рассчитать вектор скорости стрелка относительно цели
            Vector2 targetRelativeVelocity = targetVelocity - shooterVelocity;
            //рассчитать время, за которое может быть выполнен перехват
            float t = FirstOrderInterceptTime(shotSpeed, targetPosition - shooterPosition, targetRelativeVelocity);
            //получить позицию цели через заданное время
            return targetPosition + t * (targetRelativeVelocity);
        }

        /// <summary>
        /// Перехват первого порядка с использованием относительного положения цели.
        /// </summary>
        /// <param name="shotSpeed">Скорость выстрела.</param>
        /// <param name="targetRelativePosition">Позиция цели относительно стрелка.</param>
        /// <param name="targetRelativeVelocity">Скорость цели относительно стрелка.</param>
        /// <returns></returns>
        private static float FirstOrderInterceptTime(float shotSpeed, Vector2 targetRelativePosition, Vector2 targetRelativeVelocity)
        {
            //рассчитать квадрат относительной скорости
            float velocitySquared = targetRelativeVelocity.sqrMagnitude;
            if (velocitySquared < 0.001f) return 0f;

            //посчитать разницу квадратов относительной скорости цели и скорости выстрела
            float a = velocitySquared - shotSpeed * shotSpeed;

            //проверить, не совпадают ли скорости
            if (Mathf.Abs(a) < 0.001f)
            {
                float t = -targetRelativePosition.sqrMagnitude / (2f * Vector2.Dot(targetRelativeVelocity, targetRelativePosition));
                //don't shoot back in time
                return Mathf.Max(t, 0f);
            }

            float b = 2f * Vector2.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = targetRelativePosition.sqrMagnitude;
            float determinant = b * b - 4f * a * c;

            //если детерминант > 0, есть два корня
            if (determinant > 0f)
            {
                //посчитать корни
                float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a);
                float t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);

                //вернуть наименьший из положительных корней, либо 0
                if (t1 > 0f && t2 > 0f) return Mathf.Min(t1, t2);
                if (t1 > 0f) return t1;
                if (t2 > 0f) return t2;
                return 0f;
            }

            //если детерминант < 0, корня не существует - вернуть 0
            if (determinant < 0.0) return 0f;

            //если детерминант = 0, есть только один корень
            //вернуть его если он больше нуля, в противном случае вернуть 0
            return Mathf.Max(-b / (2f * a), 0f);
        }
    }
}
