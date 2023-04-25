using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public enum SpawnMode
    {
        /// <summary>
        /// Генерация сразу при старте.
        /// </summary>
        Start,
        Loop
    }

    /// <summary>
    /// Генератор сущностей.
    /// </summary>
    public class EntitySpawner : MonoBehaviour
    {
        /// <summary>
        /// Массив префабов сущностей для генерации.
        /// </summary>
        [SerializeField] private Entity[] _entityPrefabs;

        /// <summary>
        /// Зона, в которой должны генерироваться сущности.
        /// </summary>
        [SerializeField] private CircleArea _area;

        /// <summary>
        /// Режим гереации.
        /// </summary>
        [SerializeField] private SpawnMode _spawnMode;

        /// <summary>
        /// Количество одновременно генерируемых объектов.
        /// </summary>
        [SerializeField] private int _countSpawns;

        /// <summary>
        /// Период генерации.
        /// </summary>
        [SerializeField] private float _spawnPeriod;

        /// <summary>
        /// Счётчик времени генерации.
        /// </summary>
        private float _spawnTimer;

        private void Start()
        {
            if (_spawnMode == SpawnMode.Start)
            {
                //генерация сущности при старте
                SpawnEntities();
            }

            _spawnTimer = _spawnPeriod;
        }

        private void Update()
        {
            if(_spawnTimer > 0) _spawnTimer -= Time.deltaTime;
            if (_spawnMode == SpawnMode.Loop && _spawnTimer <= 0)
            {
                SpawnEntities();
                _spawnTimer = _spawnPeriod;
            }
        }

        private void SpawnEntities()
        {
            for (int i = 0; i < _countSpawns; i++)
            {
                int index = UnityEngine.Random.Range(0, _entityPrefabs.Length);
                GameObject entity = Instantiate(_entityPrefabs[index].gameObject);
                entity.transform.position = _area.GetRandomInsideArea();
            }
        }
    }
}
