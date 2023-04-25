using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Генератор космических кораблей.
    /// </summary>
    public class EntitySpawnerShips : MonoBehaviour
    {
        /// <summary>
        /// Команда кораблей, которые создаёт генератор.
        /// </summary>
        [SerializeField] private int _shipsTeam;

        /// <summary>
        /// Префабы для генерации кораблей.
        /// </summary>
        [SerializeField] private Destructible[] _shipsPrefabs;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private CircleArea _area;

        /// <summary>
        /// Количество кораблей, которые должны быть созданы.
        /// </summary>
        [SerializeField] private int _spawnCount;

        private int _shipsCount;

        /// <summary>
        /// Start запускается перед первым кадром.
        /// </summary>
        private void Start()
        {
            for (int i = 0; i < _spawnCount; i++)
            {
                SpawnShips(_shipsPrefabs, false, Vector2.zero);
            }
        }

        /// <summary>
        /// Размещение мусора на игровом поле.
        /// </summary>
        /// <param name="debrisPrefabs">Список префабов для размещения.</param>
        /// <param name="setPosition">Размещать префабы строго в заданной позиции.</param>
        /// <param name="position">Позиция размещения префабов.</param>
        private void SpawnShips(Destructible[] debrisPrefabs, bool setPosition, Vector2 position)
        {
            //выбрать префаб корабля и создать корабль
            int index = UnityEngine.Random.Range(0, debrisPrefabs.Length);
            GameObject ship = Instantiate(debrisPrefabs[index].gameObject);
            //задать команду корабля
            ship.GetComponent<SpaceShip>().TeamId = _shipsTeam;
            //добавить обработчик события уничтожения мусора
            ship.GetComponent<Destructible>().DestructionUnity.AddListener(Call);
            //ship.GetComponent<Destructible>().Destruction += OnShipDestruction;

            //задать случайную позицию корабля
            ship.transform.position = setPosition ? position : _area.GetRandomInsideArea();
            //добавить обработчик события уничтожения корабля
            ship.GetComponent<Destructible>().Destruction += OnShipDestruction;

            _shipsCount++;
            Debug.Log($"_shipsCount++ = {_shipsCount}");
        }

        private void Call()
        {
            _shipsCount--;
            Debug.Log($"_shipsCount-- = {_shipsCount}");


            //gObject.GetComponent<Destructible>().Destruction -= OnShipDestruction;
            //при уничтожении корабля нужно создать новый корабль
            while (_shipsCount < _spawnCount)
            {
                SpawnShips(_shipsPrefabs, false, Vector2.zero);
            }
        }

        /// <summary>
        /// Обработка события уничтожения мусора.
        /// </summary>
        private void OnShipDestruction(GameObject gObject)
        {
            _shipsCount--;
            Debug.Log($"_shipsCount-- = {_shipsCount}");


            gObject.GetComponent<Destructible>().Destruction -= OnShipDestruction;
            //при уничтожении корабля нужно создать новый корабль
            while (_shipsCount < _spawnCount)
            {
                SpawnShips(_shipsPrefabs, false, Vector2.zero);
            }
        }
    }
}
