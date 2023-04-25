using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Генератор космического мусора.
    /// </summary>
    public class EntitySpawnerDebris : MonoBehaviour
    {
        /// <summary>
        /// Префабы для генерации мусора.
        /// </summary>
        [SerializeField] private Destructible[] _debrisPrefabs;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private CircleArea _area;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private int _spawnCount;

        /// <summary>
        /// Скорость мусора.
        /// </summary>
        [SerializeField] private float _randomSpeed;

        private float _debrisCount;

        /// <summary>
        /// Start запускается перед первым кадром.
        /// </summary>
        private void Start()
        {
            _debrisCount = 0;
            for (int i = 0; i < _spawnCount; i++)
            {
                SpawnDebris(_debrisPrefabs, false, Vector2.zero);
            }
        }

        /// <summary>
        /// Размещение мусора на игровом поле.
        /// </summary>
        /// <param name="debrisPrefabs">Список префабов для размещения.</param>
        /// <param name="setPosition">Размещать префабы строго в заданной позиции.</param>
        /// <param name="position">Позиция размещения префабов.</param>
        private void SpawnDebris(Destructible[] debrisPrefabs, bool setPosition, Vector2 position)
        {
            //выбрать префаб мусора и создать мусор
            int index = UnityEngine.Random.Range(0, debrisPrefabs.Length);
            GameObject debris = Instantiate(debrisPrefabs[index].gameObject);
            //задать случайную позицию мусора
            debris.transform.position = setPosition ? position : _area.GetRandomInsideArea();
            //добавить обработчик события уничтожения мусора
            debris.GetComponent<Destructible>().Destruction += OnDebrisDestruction;
            //задать случайную скорость мусора
            Rigidbody2D body = debris.GetComponent<Rigidbody2D>();
            if (body != null) body.velocity = UnityEngine.Random.insideUnitSphere * _randomSpeed;
            _debrisCount++;
        }

        /// <summary>
        /// Обработка события уничтожения мусора.
        /// </summary>
        private void OnDebrisDestruction(GameObject gameObject)
        {
            _debrisCount--;

            //попытаться создать потомков уничтоженного мусора
            ChildrenSpawn childrenSpawn = gameObject.GetComponent<ChildrenSpawn>();
            if (childrenSpawn != null)
            {
                for (int i = 0; i < childrenSpawn.ChildrenCount; i++)
                {
                    SpawnDebris(childrenSpawn.ChildrenPrefabs, true, gameObject.transform.position);
                }
            }

            //при уничтожении мусора нужно создать нового мусора
            while (_debrisCount < _spawnCount)
            {
                SpawnDebris(_debrisPrefabs, false, Vector2.zero);
            }
        }
    }
}
