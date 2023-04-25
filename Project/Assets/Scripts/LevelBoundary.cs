using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public enum BoundaryMode
    {
        /// <summary>
        /// Граница уровня - непроходимая стена.
        /// </summary>
        Wall,
        /// <summary>
        /// Граница уровня телепортирует игрока на противоположную сторону.
        /// </summary>
        Teleport
    }

    public class LevelBoundary : MonoBehaviour
    {
        #region singletone
        public static LevelBoundary Instance;

        private void Awake()
        {
            //проверить, существует ли уже экземпляр этого объекта
            if (Instance != null)
            {
                //существует - самоубиться и закончить работу
                Debug.LogError("Объект LevelBoundary уже существует.");
                Destroy(gameObject);
                return;
            }

            //создать синглетон
            Instance = this;

            //запретить уничтожение синглетона при перезагрузке уровня
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        /// <summary>
        /// Радиус границы уровня.
        /// </summary>
        [SerializeField] private float _radius;
        
        /// <summary>
        /// Радиус границы уровня.
        /// </summary>
        public float Radius => _radius;

        /// <summary>
        /// Режим работы границы уровня.
        /// </summary>
        [SerializeField] private BoundaryMode _mode;

        /// <summary>
        /// Режим работы границы уровня.
        /// </summary>
        public BoundaryMode Mode => _mode;


#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward.normalized, _radius);
        }

#endif

    }
}
